unit KSOper;
interface
uses WUBase,WUKatal1,TpEntry,TpDate,WUTools,
     WUKonta,KSHelp,KSTyp,{ KSWydz v.1.48 }WuWydz;

const
      OprKodSLen   =  5;
      OprNazwaSLen = 30;
      OprMiastoSLen= 35;
      OprUlicaSLen = 25;

      OprRozrSLen  = 10;
      OprOpisSLen  = 35;

      OprRSLen = 9;
      OprNaPlus : array[boolean] of string[OprRSLen] =
                  (' wydanie ','przyjëcie');

type POprRecord     = ^OprRecord;
      OprRecord     = record
                       data  : date;
                       typ   :string[TypKodSLen];
                       kod   :string[OprKodSLen];
                       nazwa1,
                       nazwa2:string[OprNazwaSLen];
                       miasto:string[OprMiastoSLen];
                       ulica :string[OprUlicaSLen];
                       rozr  :string[OprRozrSLen];
                       opis  :string[OprOpisSLen];
                       kwota ,
                       przyj ,
                       wydan ,
                       stan  :forsa;
                       konto :string[KontoLen];
                       nrdrk :word;  { numer druku KP/KW }
                       max   :longint; { uzywane do ustalania porzadku }
                      end;

type POprBase       = ^OprBase;
      OprBase       = object(BaseSystem)
                       MR:OprRecord;
                       OdZera:boolean; { na potrzeby przeliczenia }
                       LStan : forsa;
                       constructor Init(d:date);
                       procedure PrzeliczStan;
                       procedure PrzeliczWszystkie;
                      end;

var OprBS:POprBase;

const  drkDat =$01;
       drkNr  =$02;
       drkTyp =$04;
       drkKod =$08;
       drkTyp2=$10;
type  TDrkRange=record
                   d1,
                   d2:date;
                   nr:word;
                   typ:string[TypKodSLen];
                   kod:string[OprKodSLen];
                   typ2:string[TypKodSLen];
                 end;

type POprKatal      = ^OprKatal;
      OprKatal      = object(XTKatalog)
                       p:pointer;
                       LastDay:date;
                       LastTyp:string[TypKodSLen];
                       TRWn,TRMa:Forsa;
                       constructor Init;
                       destructor Done; virtual;
                       function PiszWLine(var D): String; virtual;
                       procedure PiszWActiveLine(var D); virtual;
                       procedure PiszActiveStr(s:string);
                       function PiszWPusto: String; virtual;
                       function EditLine(Row: Byte; var D): Boolean; virtual;
                       procedure Nowy; virtual;
                       function Opis: String; virtual;
                       function EditSearchRecord(var D):boolean; virtual;
                       function DelValid:boolean; virtual;
                       procedure ExecUserCommand(Number:byte); virtual;
                       procedure Drukuj; virtual;
                       procedure KasaDruk(wz:string);
                       procedure KP;
                       procedure KW;
                       function  TagUnTagOne:integer; virtual;
                       procedure TagUnTagAll; virtual;
                       procedure TagUnTagInfo; virtual;
                      end;

procedure EditOpr;
function  GetDRange(t:string;pola:byte;var R:TDrkRange):boolean;
function FormKonto(mask,kod:string):string;

implementation
uses WUSystem,Objects,TpCrt,TpString,
     WUKatal,WUWindow,WUEntry,WUPrint,WUPrintS,
     WUKth,WUPrc,
     KSConfig,KSNom;

procedure SplitSl(s:string;sz:byte;var s1,s2,s3:string);
 var goon:boolean;
     wyraz:string;
 begin
  s1:='';s2:='';s3:='';s:=Trim(s);
  goon:=true;
  while goon and (s<>'') do begin
   if Pos(' ',s) > 0 then
     wyraz:=Copy(s,1,Pos(' ',s))
   else
     wyraz:=Copy(s,1,sz);
   if Length(s1)+Length(wyraz)<=sz then begin
    s1:=s1+wyraz;
    Delete(s,1,Length(wyraz))
   end
   else goon:=false;
  end;
  goon:=true;
  while goon and (s<>'') do begin
   if Pos(' ',s) > 0 then
     wyraz:=Copy(s,1,Pos(' ',s))
   else
     wyraz:=Copy(s,1,sz);
   if Length(s2)+Length(wyraz)<=sz then begin
    s2:=s2+wyraz;
    Delete(s,1,Length(wyraz))
   end
   else goon:=false;
  end;
  goon:=true;
  while goon and (s<>'') do begin
   if Pos(' ',s) > 0 then
     wyraz:=Copy(s,1,Pos(' ',s))
   else
     wyraz:=Copy(s,1,sz);
   if Length(s3)+Length(wyraz)<=sz then begin
    s3:=s3+wyraz;
    Delete(s,1,Length(wyraz))
   end
   else goon:=false;
  end;
 end; { SplitSl }

{-----------------------------------------------------------}
function MakeOpDtKey(var D):string; far;
var TR:OprRecord absolute D;
begin
 MakeOpDtKey:=MakeDateKey(TR.data)+MakeLKey(TR.max);
end; { MakeOpDtKey }

function MakeOpNrKey(var D):string; far;
var TR:OprRecord absolute D;
begin
 MakeOpNrKey:=chr(Ord(TypBS^.GetNaPlus(TR.typ)))+MakeWKey(TR.nrdrk);
end; { MakeOpNrKey }

const OprNAME  = 'Opr';
      OprKEYNO = 2;
      OprKEYTAB:array[1..OprKEYNO]of InitIndex_type = (
       ( KSize:SizeOf(Date)+SizeOf(longint); Dupl:Duplicates; MakeKeyPtr:@MakeOpDtKey),
       ( KSize:1+SizeOf(Word); Dupl:Duplicates; MakeKeyPtr:@MakeOpNrKey)
       );

constructor OprBase.Init(d:date);
var  dd,m,y:integer;
begin
 DateToDMY(d,dd,m,y);
 if not inherited Init(OprNAME+long2str(y),sizeof(OprRecord),OprKEYNO,OprKEYTAB,1,@MR) then fail;
 SetSearchPattern(1,CharStr('N', SizeOf(Date)+SizeOf(longint)) );
 SetSearchPattern(2,CharStr('N', 1+SizeOf(Word)) );
 OdZera:=true;
end; { Init }

const txtPrzeliczMsg = 'Obliczam stan kasy. Czekaj cierpliwie ...';

procedure OprBase.PrzeliczStan;
var stan:forsa;
    bs:basestate;
begin
  Write24(txtPrzeliczMsg,_H);
  SaveBase(bs);
  if OdZera then
    stan := 0
  else begin
    stan:=OprBS^.MR.stan;
    OprBS^.Next;
  end;
  OprBS^.SetFlushing(False);
  while OprBS^.Pass and not OprBS^.EmptyBase do begin
    if TypBS^.GetNaPlus(OprBS^.MR.typ) then
      stan := stan + OprBS^.MR.kwota
    else
      stan := stan - OprBS^.MR.kwota;
    OprBS^.MR.stan := stan;
    OprBS^.FlushRec;
    OprBS^.Next;
  end;
  OprBS^.SetFlushing(True);
  if not OprBS^.EmptyBase and (OprBS^.MR.data=CurrDate) then
    LStan:=OprBS^.MR.stan;
  RestoreBase(bs);
end; { PrzeliczStan }

procedure OprBase.PrzeliczWszystkie;
var stan:forsa;
    bs:basestate;
begin
  Write24(txtPrzeliczMsg,_H);
  SaveBase(bs);
  stan:=0;
  Open;
  if not Pass then begin restoreBase(bs); exit; end;
  SetFlushing(Off); SetPrefixTo(''); Select(1);
  First;
  while Pass do
   begin
    if TypBS^.GetNaPlus(MR.typ) then
      stan := stan + MR.kwota
    else
      stan := stan - MR.kwota;
    MR.stan := stan;
    FlushRec;
    Next;
   end;
  SetFlushing(On);
  RestoreBase(bs);
end; { PrzeliczWszystkie }

{++++++++++++++++ katalog ++++++++++++++++++}
const  KatOpisSLen = 18;
       ALLen=78;
       ALCol=2;  ALRow=2;
constructor OprKatal.Init;
const LCol = 1;
var s:string;
    RCol:byte;
begin
 RCol:=LCol+DateSLen+1+TypKodSLen+1+KatOpisSLen+3*(ForsaSLen+1)+1;
 s:=' Operacje kasowe: ';
 {$IFDEF DEMO}
   s:=s+'(wer. DEMO) ';
 {$ELSE}
   s:=s+CFG.Przeds+' ';
 {$ENDIF}
 s:=Copy(s,1,RCol-LCol-1);
 if not inherited Init(s,OprBS,hlpOprKat) then fail;
 if not SaveWindow(ALCol,ALRow,ALCol+ALLen-1,ALRow,true,p) then Fail;
 EnableCommand(cmdWydruk);
 CmdTab[cmdWydruk].Nazwa := '~R~aport';
 EnableCommand(cmdSzukaj);
 SetWindow(LCol,3,RCol,23);
 OprBS^.Last;
 if OprBS^.Pass then begin
   OprBS^.LStan := OprBS^.MR.stan;
   LastDay:=OprBS^.MR.data;
   LastTyp:=OprBS^.MR.typ;
 end
 else begin
   OprBS^.LStan := 0 ;
   LastDay:=0;
   LastTyp:='';
 end;
 { kumsztryk dla zdalnego wywolywania przlieczenia stanu }
 EnableCommand(cmdUser1);
 CmdTab[cmdUser1].Nazwa := '~K~P';
 EnableCommand(cmdUser2);
 CmdTab[cmdUser2].Nazwa := 'K~W~';
 EnableCommand(cmdUser3);
 CmdTab[cmdUser3].Nazwa := 'prze~L~icz';
 EnableCommand(cmdUser4);  { zmiana daty }
 CmdTab[cmdUser4].Nazwa := HIDELABEL+CODELABEL+long2Str(_kbAltD);
 CmdTab[cmdUser4].OnEmptyBase := true;
 EnableCommand(cmdUser5);  { przelicz wszystkie }
 CmdTab[cmdUser5].Nazwa := HIDELABEL+CODELABEL+long2Str(_kbCtrL);
 EnableCommand(cmdTU);
 EnableCommand(cmdTUAll);
 EnableCommand(cmdTUInfo);
 TRWn:=0;
 TRMa:=0;
end; { Init }

destructor OprKatal.Done;
begin
 RestoreWindow(ALCol,ALRow,ALCol+ALLen-1,ALRow,true,p);
 inherited Done;
end;

function OprKatal.PiszWLine(var D):string;
var TR:OprRecord absolute D;
    s:string;
begin
 if TypBS^.GetNaPlus(TR.typ) then
   s:=FormForsa(TR.kwota)+'≥'+CharStr(' ',ForsaSLen)
 else
   s:=CharStr(' ',ForsaSLen)+'≥'+FormForsa(TR.kwota);
 s:=s+'≥';
 PiszWLine:=Pad(Copy(Date2Str(TR.data)+'≥'+
                     Pad(TR.typ,TypKodSLen)+'≥'+
                     Pad(Copy(TR.opis,1,KatOpisSLen),KatOpisSLen)+'≥'+
                     S+
                     FormForsa(TR.stan)
                     ,1,X2-X1-1),X2-X1-1);
end; { PiszWLine }

procedure OprKatal.PiszActiveStr(s:string);
begin
  FastWrite(Center(Copy(s,1,ALLen),ALLen),ALRow,ALCol,_I);
end; { PiszActiveStr }

procedure OprKatal.PiszWActiveLine(var D);
var TR:OprRecord absolute D;
begin
  PiszActiveStr(TR.nazwa1+' '+TR.nazwa2+' '+TR.miasto);
end; { OprKatal.PiszWActiveLine }

function OprKatal.PiszWPusto:string;
begin
 PiszWPusto:=Pad(CharStr(' ',DateSLen)+'≥'+
                 CharStr(' ',TypKodSLen)+'≥'+
                 CharStr(' ',KatOpisSLen)+'≥'+
                 CharStr(' ',ForsaSLen)+'≥'+
                 CharStr(' ',ForsaSLen)+'≥'+
                 CharStr(' ',ForsaSLen)
                 ,X2-X1-1);
end; { PiszWPusto }

function OprKatal.Opis:string;
begin
 Opis:=Center('data',DateSLen)+' '+
       Center('t.',TypKodSLen)+' '+
       Center('treûç',KatOpisSLen)+' '+
       Center('przych¢d',ForsaSLen)+' '+
       Center('rozch¢d',ForsaSLen)+' '+
       Center('stan',ForsaSLen);
end; { Opis }

const
       EdTyp='TYP';
       EdTNz='TNZ';
       EdKod='KOD';
       EdNz1='NZ1';
       EdNz2='NZ2';
       EdMst='MST';
       EdUlc='ULC';
       EdTsc='TSC';
       EdKWT='KWT';
       EdIN ='IN';
       EdOUT='OUT';
       EdKnt='KNT';

type POprESR = ^OprESR;
      OprESR = object( WindowESR )
                 NaPlus:boolean;
                 Katal :KatalSet;
                 konto :string[KontoLen];
                 procedure PreEdit; virtual;
                 procedure PostEdit; virtual;
               end;

procedure OprESR.PreEdit;
const EDITNOMLINE      = '~F1~ - pomoc           ~F3~ - nominaíy           ~Esc~ - porzucenie edycji';
      EDITKATLINE      = '~F1~ - pomoc       ~F2~ - katalog        ~Esc~ - porzucenie edycji';
begin
 AddEntryCommand(ESNone, 1,_kbF2, 0);
 AddEntryCommand(ESNone, 1,_kbF3, 0);
 if ESR.CurrentId=GetFieldId(EdKWT) then begin
   AddEntryCommand(ESUser1, 1,_kbF3, 0);
   WriteMenu(CenterTylda(EDITNOMLINE,78),24,2,_N,_UH);
 end
 else
   if (ESR.CurrentId=GetFieldId(EdKod)) and
      (Katal<>ktNone) then begin
     {$IFDEF IMPKTH}
     if Katal=ktKth then
       KthBS^.ImportKth;
     {$ENDIF}
     AddEntryCommand(ESUser0, 1,_kbF2, 0);
     WriteMenu(CenterTylda(EDITKATLINE,78),24,2,_N,_UH);
   end
   else
     inherited PreEdit;
end; { PreEdit }

function FormKonto(mask,kod:string):string;
var i,p,il:byte;
begin
  i:=Length(mask);il:=0;p:=0;
  while i>1 do begin
    if mask[i]='K' then begin
      Inc(il);
      p:=i;
    end;
    Dec(i);
  end;
  if (p>0) and (il>0) then begin
    Delete(mask,p,il);
    if Length(Kod)>il then
      Kod:=Copy(Kod,Length(Kod)-il+1,Il);
    Insert(Kod,mask,p);
  end;
  FormKonto:= Mask;
end; { FormKonto }

procedure OprESR.PostEdit;
var kwt,f1:forsa;
    jakie,cf,
    s1,s2,s3:string;
    FRP:FieldRecPtr;
    wk:word;
begin
  if (LastEntryCommand<>ESQuit)  then begin
    if LastEntryCommand=esUser1 then begin
      GetField(GetCurrentFieldName,kwt);
      Nominaly(kwt);
      LastEntryCommand:=esNone;
    end; { esUser1 }
    { dalsze }
    cf:=GetCurrentFieldName;
    if cf=EdTyp then begin
      GetField(EdTyp,jakie);
      NaPlus:=TypBS^.GetNaPlus(jakie);
      Katal :=TypBS^.GetKatal(jakie);
      konto :=TypBS^.GetKonto(jakie);
      FastWrite(TypNaPlus[NaPlus],
                ESR.CurrentField^.eRow,
                ESR.CurrentField^.eCol+TypKodSLen+2,_H);
      FRP:=FindFieldId(ESR,GetFieldId(EdKod));
      FRP^.ValidatePtr:=KatProcPtr[Katal];
      ChangeRequired(ESR,GetFieldId(EdKod),(Katal<>ktNone));
      s1:=TypBS^.GetOpis(jakie);
      if UpDateField(EdTNz,s1) then
        DrawField(ESR,GetFieldId(EdTNz));
      if CurrentFieldModified(ESR) then begin
        GetField(EdTsc,s2);
        if (s2='') and UpDateField(EdTsc,s1) then
          DrawField(ESR,GetFieldId(EdTsc));
      end;
      s1:='';
      if UpDateField(EdKnt,s1) then
        DrawField(ESR,GetFieldId(EdKnt));
    end;
    if (cf=EdKod) then begin
      GetField(EdKod,jakie);
      case Katal of
        ktNone:begin
                 { oraz konta }
                 s1:=FormKonto(konto,jakie);
                 GetField(EdKnt,s2);
                 if Trim(s2)='' then
                   if UpDateField(EdKnt,s1) then
                     DrawField(ESR,GetFieldId(EdKnt));
               end;
        ktKth:begin
                { podpowiedzenie nazwy i adresu }
                KthBS^.Open;KthBS^.Select(1);
                KthBS^.Find(jakie);
                if KthBS^.Pass then begin
                  SplitSl(KthBS^.MR.nazwa,OprNazwaSLen,s1,s2,s3);
                  if UpDateField(EdNz1,s1) then
                    DrawField(ESR,GetFieldId(EdNz1));
                  if UpDateField(EdNz2,s2) then
                    DrawField(ESR,GetFieldId(EdNz2));
                  s1:=KthBS^.MR.poczta+' '+KthBS^.MR.miasto;
                  if UpDateField(EdMst,s1) then
                    DrawField(ESR,GetFieldId(EdMst));
                  s1:=KthBS^.MR.ulica;
                  if UpDateField(EdUlc,s1) then
                    DrawField(ESR,GetFieldId(EdUlc));
                end;
                { oraz konta }
                s1:=FormKonto(konto,jakie);
                GetField(EdKnt,s2);
                if Trim(s2)='' then
                  if UpDateField(EdKnt,s1) then
                    DrawField(ESR,GetFieldId(EdKnt));
              end;
        ktPrc:if Str2Word(jakie,wk) then begin
                { podpowiedzenie nazwiska i imienia }
                s1:=PrcBS^.GetNazw(wk);
                if UpDateField(EdNz1,s1) then
                  DrawField(ESR,GetFieldId(EdNz1));
                s1:=PrcBS^.GetImie(wk);
                if UpDateField(EdNz2,s1) then
                  DrawField(ESR,GetFieldId(EdNz2));
                { oraz konta }
                s1:=FormKonto(konto,jakie);
                GetField(EdKnt,s2);
                if Trim(s2)='' then
                  if UpDateField(EdKnt,s1) then
                    DrawField(ESR,GetFieldId(EdKnt));
              end;
        ktWdz:begin
                { podpowiedzenie nazwy }
                s1:=WdzBS^.GetNazwa(jakie);
                if UpDateField(EdNz1,s1) then
                  DrawField(ESR,GetFieldId(EdNz1));
                { oraz konta }
                s1:=FormKonto(konto,jakie);
                GetField(EdKnt,s2);
                if Trim(s2)='' then
                  if UpDateField(EdKnt,s1) then
                    DrawField(ESR,GetFieldId(EdKnt));
              end;
      end;
    end;
    GetField(EdKWT,kwt);
    { ustalenie domyûlnej po kwocie }
    if CF=EdKwt then begin
      if NaPlus then
        jakie:=EdIn
      else
        jakie:=EdOut;
      GetField(jakie,f1);
      if f1=0 then begin
        if UpDateField(jakie,kwt) then
          DrawField(ESR,GetFieldId(jakie));
      end;
    end; { kwota }
    { wyliczenia reszty }
    if NaPlus then begin
      GetField(EdIn,f1);
      f1:=f1-kwt;
      if f1>=0 then begin
        if UpDateField(EdOut,f1) then
          DrawField(ESR,GetFieldId(EdOut));
      end;
    end
    else begin
      GetField(EdOut,f1);
      f1:=f1-kwt;
      if f1>=0 then begin
        if UpDateField(EdIn,f1) then
          DrawField(ESR,GetFieldId(EdOut));
      end;
    end
  end; { not ESQuit }
end; { PostEdit }

function OprKatal.EditLine(Row: Byte; var D): Boolean;
var PE:POprESR;
    TR:OprRecord absolute D;
    PopKwota:forsa;
    Przeliczenie:boolean;
    dummy:boolean;
    PTK:PTypKat;
    PKK:PKntKat;
    tnz:string;
    nowyrec:boolean;
begin
 EditLine:=false;
 PE:=nil; New(PE,Init(15,4,67,22));
 if PE=nil then begin Message(txtNOHEAP); exit end;
 PTK:=nil;
 New(PTK,Init);
 if PTK=nil then begin
   Message(txtNoHeap);
   Dispose(PE,Done);
   Exit
 end;
 PKK:=nil;
 New(PKK,Init);
 if PKK=nil then begin
   Message(txtNoHeap);
   Dispose(PE,Done);
   Dispose(PTK,Done);
   Exit
 end;
 with PE^ do
  begin
   W^.Title:=NewStr(' Operacja ');
   LastHelpNo:=hlpOprEdit;
   SetRequired(On);
   if TR.data=0 then begin
     TR.data := CurrDate;
     TR.stan := OprBS^.LStan;
     { TR.typ  := LastTyp; }
     tnz:='';
     Przeliczenie :=false;
     nowyrec:=true;
   end
   else begin
     tnz:=TypBS^.GetOpis(TR.typ);
     Przeliczenie := (TR.data=CurrDate);
     nowyrec:=false;
   end;
   PopKwota:=TR.kwota;
   SetProtection(On);
   AddAutoField('DT',ftDate,      'Data        : ',DateEPic,nil,TR.data);
   SetProtection(Off);
   SetRequired(Off);
   AddAutoField(EdTyp,ftDirCheck, 'Typ         : ',CharStr('!',TypKodSLen),
                 PTK,TR.typ);
   SetProtectAttrs(_H,_H);
   SetProtection(On);
   AddAutoField(EdTNz,ftString  , '',CharStr('X',TypNameSLen),nil,tnz);
   SetProtection(Off);
   SetProtectAttrs(_N,_N);
   AddAutoField(EdKod,ftString  , 'Klient - kod: ',CharStr('X',OprKodSLen),
                 nil,TR.kod);
   AddAutoField(EdNz1,ftString,   '  nazwa     : ','!'+CharStr('X',OprNazwaSLen-1),
                 nil,TR.nazwa1);
   AddAutoField(EdNz2,ftString,   '              ','!'+CharStr('X',OprNazwaSLen-1),
                 nil,TR.nazwa2);
   AddAutoField(EdMst,ftString,   '  miasto    : ','!'+CharStr('X',OprMiastoSLen-1),
                 nil,TR.miasto);
   AddAutoField(EdUlc,ftString,   '  ulica     : ','!'+CharStr('X',OprUlicaSLen-1),
                 nil,TR.ulica);
   {SetRequired(On);}
   AddAutoField('RZR',ftString,   'Rozrachunek : ',CharStr('!',OprRozrSLen),
                 nil,TR.rozr);
   SetRequired(Off);
   AddAutoField(EDTsc,ftString,   'Treûç       : ','!'+CharStr('X',OprOpisSLen-1),
                 nil,TR.opis);
   if TR.data <> CurrDate then
     SetProtection(On);
   SetNumeric(On);
   AddAutoField(EdKwt,ftForsa,    'Kwota       : ','',
                 nil,TR.kwota);
   AddAutoField(EdIn, ftForsa,    'Przyjëto    : ','',
                 nil,TR.przyj);
   AddAutoField(EdOut,ftForsa,    'Wydano      : ','',
                 nil,TR.wydan);
   SetNumeric(Off);
   SetProtection(Off);
   AddAutoField(EdKnt ,ftDir,     'Konto       : ',CharStr('X',KontoLen),
                 PKK,TR.konto);
   if not Supervisor then SetProtection(On);
   AddAutoField('NRDK',ftWord  ,  'Nr druku    : ','99999',
                 nil,TR.nrdrk);
   SetProtection(Off);
   if Execute then begin
     EditLine:=True;
     LastTyp:=TR.typ;
     if TypBS^.GetNaPlus(TR.typ) then
       TR.stan := TR.stan - PopKwota + TR.kwota
     else
       TR.stan := TR.stan + PopKwota - TR.kwota;
     if nowyrec then begin
       if CFGBS.Reload then begin
         Inc(CFG.LastNr);
         if CFGBS.Update then
           TR.max:=CFG.LastNr
         else begin
           Przeliczenie:=false;
           Message('BíÜd zapisu operacji'^G);
           EditLine:=false;
         end;
       end
       else begin
         Przeliczenie:=false;
         Message('BíÜd zapisu operacji'^G);
         EditLine:=false;
       end;
     end;
     if Przeliczenie then begin
       OprBS^.OdZera:=false;
       StuffKey(_kbL);
     end;
   end;
  end;
 Dispose(PKK,Done);
 Dispose(PTK,Done);
 Dispose(PE,Done);
end; { EditLine }

procedure OprKatal.Nowy;
var nrr:longint;
begin
  {$IFDEF DEMO}
  if DB^.AllRecs>15 then begin
    Message('W wersji DEMO nie moßesz juß nic dopisaç'^G);
    Exit
  end;
  {$ENDIF}
  nrr:=OprBS^.NrRec; { <- na potrzeby przelicze§ }
  inherited Nowy;
  if (nrr<>OprBS^.NrRec) and (OprBS^.NrRec<>0) then begin
    { rekord zostaí dopisany }
    if OprBS^.MR.data < LastDay then begin
      OprBS^.Previus;
      OprBS^.OdZera:=(not OprBS^.Pass);
      if not OprBS^.Pass then
        OprBS^.Select(OprBS^.NrMainIndex);
      OprBS^.PrzeliczStan
    end
    else
      OprBS^.LStan := OprBS^.MR.stan;
  end;
end; { Nowy }

function OprKatal.DelValid:boolean;
var bs:BaseState;
    nrr:longint;
    k:forsa;
begin
  DelValid := False;
  if OprBS^.MR.data <> CurrDate then begin
    Message('Nie moßesz usunÜç tej operacji !!!');
    Exit;
  end;
  OprBS^.SaveBase(bs);
  OprBS^.Select(OprBS^.NrMainIndex);
  OprBS^.Previus;
  if OprBS^.Pass then begin
    nrr:=OprBS^.NrRec;
    OprBS^.Next;
  end
  else
    nrr:=0;
  if inherited DelValid then begin
    if TypBS^.GetNaPlus(OprBS^.MR.typ) then
      k:=-OprBS^.MR.kwota
    else
      k:= OprBS^.MR.kwota;
    OprBS^.Delete;
    PiszActiveStr('');
    if not OprBS^.EmptyBase and
       (OprBS^.NrRec<>0) and
       (OprBS^.NrRec<>nrr) then begin
      { baza niepusta i skasowano nie-ostatni rekord }
      OprBS^.SaveBase(bs);
      if nrr>0 then begin
        OprBS^.SeekTo(nrr);
        OprBS^.Select(OprBS^.NrMainIndex);
        OprBS^.OdZera:=(Abs(OprBS^.MR.stan)=OprBS^.MR.kwota);
      end
      else begin
        OprBS^.MR.stan := OprBS^.MR.stan + K;
        OprBS^.FlushRec;
        OprBS^.OdZera:=true;
      end;
      OprBS^.PrzeliczStan;
      OprBS^.RestoreBase(bs);
    end
    else
      OprBS^.LSTan:=OprBS^.LStan+K;
  end;
end; { DelValid (Delete) }

const  KPorKWTable:array[boolean] of string[4] =
                   (' KW ',' KP ');
procedure KPorKW(var Value; ID:byte;
                    Factor:integer; var S:string); far;
var BoolValue:boolean absolute value;
begin
 if Abs(Factor)<>0 then BoolValue:=not BoolValue;
 S:=KPorKWTable[BoolValue];
end; { KPorKW }

function OprKatal.EditSearchRecord(var D):boolean;
var TR:OprRecord absolute D;
    PE:PESRObject;
    b,fn:boolean;
begin
 EditSearchRecord := false;
 FillChar(TR,sizeof(TR),#0);
 FastWrite(CharStr(' ',78),24,2,NormalAttr);
 PE:=nil; New(PE,Init);
 if PE=nil then begin Message(txtNOHEAP); exit end;
 with PE^ do
  begin
   SetPreEditPtr(ESR,nil);
   LastHelpNo:=hlpOprSrch;
   case DB^.NrMainIndex of
    1: begin{ data }
         TR.data := OprBS^.MR.data;
         SetClearFirstChar(Off);
         AddStandardField('DT',ftDate,'Podaj szukanÜ datë :',24,10,
                          DateEPic,0,0,nil,TR.data);
         SetClearFirstChar(On);
       end;
    2: begin
         b:=false;
         AddStandardField('NP',ftChoice,'Podaj szukany typ :',24,10,
                       CharStr('X',4),0,0,@KPorKW,b);
         AddStandardField('NRDK',ftWord  ,' i nr druku :',LastRow,LastCol,
                       '99999',0,0,nil,TR.nrdrk);
       end;
   end; { of case }
   if Execute then begin
     EditSearchRecord:=true;
     if DB^.NrMainIndex=2 then begin
       TypBS^.First;fn:=false;
       while TypBS^.Pass and not fn do begin
         if TypBS^.MR.naplus=b then begin
           fn:=true;
           TR.typ:=TypBS^.MR.kod;
         end;
         TypBS^.Next;
       end;
     end;
   end
   else
     EditSearchRecord:=false
  end;
 Dispose(PE,Done);
end; { EditSearchRecord }

{******************** obiekt na potrzeby druk¢w kasowych ***********}
type
  PKasaDruk = ^KasaDruk;
  KasaDruk = object( CorSer )
    Kwota: forsa;
    KKod,
    Nazwa1,Nazwa2,
    Miasto,Ulica,
    Tresc,Rozr,
    slownie1,
    slownie2,
    slownie3 : String;
    data:date;
    Nr,PzNr:word;
    constructor Init(WZName:string);
    function Translate(var Rec; S: String; var Result: String): Boolean;
      virtual;
  end;

constructor KasaDruk.Init(WZName:string);
begin
  CorSer.Init(WZName+'.WZ');
  if ErrCode <> 0 then begin
    Message('BíÜd odczytu wzorca wydruku '+WZName+''^G);
    Fail
  end;
  slownie1:='';slownie2:='';slownie3:='';
end; { KasaDruk.Init }

function KasaDruk.Translate(var Rec; S: String; var Result: String): Boolean;
var
  UpS: String;
  Len: Byte;
  DD,MM,YY:Integer;
begin
  Translate := True;
  if CorSer.Translate(Rec, S, Result) then
    Exit;
  TransLen(S, UpS, Len);

       if UpS = 'NR'  then Result := LeftPad(long2str(Nr),Len)
  else if UpS = 'KLKOD'  then Result := Pad(S2P(KKOd),Len)
  else if UpS = 'NAZWA1'  then Result := Pad(S2P(Nazwa1),Len)
  else if UpS = 'NAZWA2'  then Result := Pad(S2P(Nazwa2),Len)
  ELSE if UpS = 'MIASTO' then Result := Pad(S2P(Miasto),Len)
  ELSE if UpS = 'ULICA'  then Result := Pad(S2P(ulica),Len)
  else if UpS = 'KWOTA'  then Result := LeftPad(Trim(FormForsa(Kwota)),Len)
  else if UpS = 'DATA' then Result := LeftPad(Trim(Date2Str(Data)),Len)
  else if UpS = 'RK'  then begin
         DateToDMY(Data,DD,MM,YY);
         Result := Pad(long2str(YY mod 100),Len)
       end
  else if UpS = 'MC'  then begin
         DateToDMY(Data,DD,MM,YY);
         Result := Pad(long2str(MM),Len)
       end
  else if UpS = 'TRESC'  then Result := Pad(S2P(Tresc),Len)
  else if UpS = 'ROZRACH' then Result := Pad(S2P(Rozr),Len)
  else if UpS = 'SLOWNIE1'then Result := Pad(S2P(slownie1),Len)
  else if UpS = 'SLOWNIE2'then Result := Pad(S2P(slownie2),Len)
  else if UpS = 'SLOWNIE3'then Result := Pad(S2P(slownie3),Len)
  else if UpS = 'RPNR' then Result := LeftPad(long2str(CFG.RpNr),Len)
  else if UpS = 'PZNR' then Result := LeftPad(long2str(PzNr),Len)
  ELSE if UpS = 'CFGNAZWA' then Result := Pad(S2P(Cfg.Przeds),Len)
  ELSE if UpS = 'CFGMIASTO' then Result := Pad(S2P(Cfg.Miasto),Len)
  ELSE if UpS = 'CFGKOD' then Result := Pad(S2P(Cfg.Kod),Len)
  ELSE if UpS = 'CFGULICA' then Result := Pad(S2P(Cfg.Ulica),Len)
  else begin
    Result := Pad('', Len);
    Translate := False;
    Exit;
  end;
  if Options and csLeftPad <> 0 then begin
   Result:=LeftPad(Trim(Result),Len);
  end;
  Translate := True;
end; { KasaDruk.Translate }


procedure OprKatal.KasaDruk(wz:string);
var  PKPDrk:PKasaDruk;
     bs:BaseState;
     newnr:word;
     ps:string;
     pnrr:longint;
begin
  PKPDrk:=nil;
  New(PKPDrk,Init(wz));
  if PKPDrk=nil then Exit;
  if PKPDrk^.Prn^.PrintYesNoQn('RozpoczÜç druk '+wz+' ?') then begin
    Write24('Drukujë '+wz+'. Czekaj cierpliwie ...',_H);
    OprBS^.LockBase;
    if not OprBS^.Pass then begin
      Dispose(PKPDrk,Done);
      Exit;
    end;
    pnrr:=OprBS^.NrRec;
    OprBS^.SaveBase(bs);
    if (OprBS^.MR.nrdrk=0) then begin
      OprBS^.Select(2);
      OprBS^.SetPrefixTo(chr(Ord(TypBS^.GetNaPlus(OprBS^.MR.typ))));
      OprBS^.Last;
      if OprBS^.Pass then
        newnr:=Succ(OprBS^.MR.nrdrk)
      else
        newnr:=1;
      OprBS^.RestoreBase(bs);
      OprBS^.MR.nrdrk:=newnr;
      OprBS^.FlushRec;
    end;
    OprBS^.Select(1);
    OprBS^.SetPrefixTo('');
    { ustalenie numeru pozycji w raporcie }
    ps:=MakeDateKey(CFG.rpdt1);
    OprBS^.Search(ps);
    PKPDrk^.pznr:=1;
    while OprBS^.Pass and (OprBS^.NrRec<>pnrr) do begin
      Inc(PKPDrk^.pznr);
      OprBS^.Next;
    end;
    if not OprBS^.Pass then
      PKPDrk^.pznr:=0;
    OprBS^.RestoreBase(bs);
    OprBS^.UnLockBase;
    with OprBS^.MR do begin
      PKPDrk^.nr:=nrdrk;
      PKPDrk^.data:=Data;
      PKPDrk^.KKod:=Kod;
      PKPDrk^.Nazwa1:=Nazwa1;
      PKPDrk^.Nazwa2:=Nazwa2;
      PKPDrk^.Miasto:=Miasto;
      PKPDrk^.Ulica :=Ulica;
      PKPDrk^.Tresc :=Opis;
      PKPDrk^.rozr  :=Rozr;
      PKPDrk^.Kwota :=Kwota;
      SplitSl(Slownie(Kwota)+' zí.',50,
              PKPDrk^.Slownie1,PKPDrk^.Slownie2,PKPDrk^.Slownie2);
    end;
    PKPDrk^.SnglPrint(wz);
  end;
  Dispose(PKPDrk,Done);
end;

procedure OprKatal.KP;
begin
  if not TypBS^.GetNaPlus(OprBS^.MR.typ) then begin
    Message('Nie moßesz drukowaç KP dla wydania !'^G);
    Exit;
  end;
  KasaDruk('KP');
end; { KP }

procedure OprKatal.KW;
begin
  if TypBS^.GetNaPlus(OprBS^.MR.typ) then begin
    Message('Nie moßesz drukowaç KW dla przyjëcia !'^G);
    Exit;
  end;
  KasaDruk('KW');
end; { KP }

function NrRows(w:word):byte;
var bb:byte;
    bg:word;
begin
  bb:=0;
  if (w and drkDat) <> 0 then Inc(bb,2);
  if (w and drkNr ) <> 0 then Inc(bb);
  if (w and drkTyp) <> 0 then Inc(bb);
  if (w and drkTyp2) <> 0 then Inc(bb,2);
  NrRows:=bb;
end; { NrRows }

const MaxERPLen=15;

function MERP(s:string):string;
begin
  MERP:=Pad(s,MaxERPLen)+' :';
end; { MERP }

const  RgKod='KOD';
       RgTyp='TYP';
       RgTyp1='TYP1';
       RgTyp2='TYP2';
type PRngESR = ^RngESR;
      RngESR = object( WindowESR )
                 Katal :KatalSet;
                 procedure PreEdit; virtual;
                 procedure PostEdit; virtual;
               end;

procedure RngESR.PreEdit;
const EDITKATLINE      = '~F1~ - pomoc       ~F2~ - katalog        ~Esc~ - porzucenie edycji';
begin
 AddEntryCommand(ESNone, 1,_kbF2, 0);
 if (ESR.CurrentId=GetFieldId(RgKod)) and
    (Katal<>ktNone) then begin
   AddEntryCommand(ESUser0, 1,_kbF2, 0);
   WriteMenu(CenterTylda(EDITKATLINE,78),24,2,_N,_UH);
 end
 else
   inherited PreEdit;
end; { PreEdit }

procedure RngESR.PostEdit;
var kwt,f1:forsa;
    jakie,cf,
    s1,s2,s3:string;
    FRP:FieldRecPtr;
    wk:word;
begin
  if (LastEntryCommand<>ESQuit)  then begin
    cf:=GetCurrentFieldName;
    if cf=RgTyp then begin
      GetField(RgTyp,jakie);
      Katal :=TypBS^.GetKatal(jakie);
      FRP:=FindFieldId(ESR,GetFieldId(RgKod));
      FRP^.ValidatePtr:=KatProcPtr[Katal];
    end;
  end; { not ESQuit }
end; { PostEdit }

function  GetDRange(t:string;pola:byte;var R:TDrkRange):boolean;
const  lf=26; rt= lf+20+DateELen;
var  PE:PRngESR;
     DD,MM,YY:Integer;
     DC1,DC2:Date;
     PF:FieldRecPtr;
     ile,tp,bt:byte;
     PTK:PTypKat;
begin
  GetDRange:=false;
  ile:=NrRows(Pola);
  if ile=0 then Exit;
  tp:=(22 - (ile+3)) div 2;
  bt:=tp+ile+3;
  FillChar(R,SizeOf(R),#0);
  PE:=nil; New(PE,Init(lf,tp,rt,bt));
  if PE=nil then begin Message(txtNOHEAP); exit end;
  PTK:=nil;
  with PE^ do begin
    W^.Title:=NewStr(' '+t+' ');
    LastHelpNo:=hlpDrkEdit;
    if (Pola and drkNr)<>0 then begin
      R.nr:=CFG.rpnr;
      AddAutoField('nr',ftWord,MERP('numer'),CharStr('9',4),nil,R.nr);
    end;
    SetClearFirstChar(Off);
    if (Pola and drkDat)<>0 then begin
      R.D2:=CurrDate;
      DateToDMY(R.D2,DD,MM,YY);
      R.D1:=DMYToDate(1,MM,YY);
      if ((Pola and drkNr)<>0) and (R.nr>0) then
        R.D1:=CFG.rpdt1;
      DC1:=DMYToDate(1,1,YY);
      DC2:=DMYToDate(31,12,YY);
      AddAutoField('DT1',ftDate,MERP('od dnia'),DateEPic,nil,R.D1);
      PF:=FindFieldId(PE^.ESR,PE^.GetFieldId('DT1'));
      PF^.RangeLo.rtDat:=DC1;
      PF^.RangeHi.rtDat:=DC2;
      AddAutoField('DT2',ftDate,MERP('do dnia'),DateEPic,nil,R.D2);
      PF:=FindFieldId(PE^.ESR,PE^.GetFieldId('DT2'));
      PF^.RangeLo.rtDat:=DC1;
      PF^.RangeHi.rtDat:=DC2;
    end;
    if (Pola and drkTyp)<>0 then begin
      New(PTK,Init);
      if PTK=nil then begin
        Dispose(PE,Done);
        Exit;
      end;
      AddAutoField(RgTyp,ftDirCheck,MERP('typ operacji'),
                   CharStr('!',TypKodSLen),PTK,R.typ);
    end;
    if (Pola and drkKod)<>0 then
      AddAutoField(RgKod,ftString  ,MERP('kod klienta'),CharStr('X',OprKodSLen),
                   nil,R.kod);
    if (Pola and drkTyp2)<>0 then begin
      New(PTK,Init);
      if PTK=nil then begin
        Dispose(PE,Done);
        Exit;
      end;
      AddAutoField(RgTyp1,ftDir,MERP('operacje: od'),
                   CharStr('!',TypKodSLen),PTK,R.typ);
      AddAutoField(RgTyp2,ftDir,MERP('          do'),
                   CharStr('!',TypKodSLen),PTK,R.typ2);
    end;
    GetDRange := Execute;
  end;
  if PTK<>nil then
    Dispose(PTK,Done);
  Dispose(PE,Done);
end; { GetDRange }

type   PRapKasowy = ^RapKasowy;
        RapKasowy = object(BFBaseReaktor)
                   Rg:TDrkRange;
                   Cn:word;
                   EditOk:boolean;
                   constructor Init(PBS:PBaseSystem);
                   function FormIntSum(Index:integer;N:IntNumber):string; virtual;
                   function FormLine(var D):PChar; virtual;
                 end;

const  LPLen=4;  Sep = '|';
       KlnLen=54;
constructor RapKasowy.Init(PBS:PBaseSystem);
var hds:string;
begin
  if not inherited Init(PBS,1) then Fail;
  EditOk := GetDRange('Raport kasowy',drkDat or drkNr,Rg);
  if not EditOk then Exit;
  hds:=Center('L.P.',LPLen)+Sep+
       Center('Data',DateSLen)+Sep+
       Center('Dow¢d',oprRozrSLen)+Sep+
       Center('Klient/Treûç',KlnLen)+Sep+
       Center('Przych¢d',ForsaSLen)+Sep+
       Center('Rozch¢d',ForsaSLen)+Sep+
       Center('Nr',LPLen)+Sep+
       Center('Konto',KontoLen);

  if not ResetWidth(Length(hds)) then Fail;

  if not AddStrLine(ptHd,Center(WalorName,PrnWidth)) then Fail;
  if not AddStrLine(ptHd,Pad(CFG.Przeds,PrnWidth-13)+
                              'Dn. @D') then Fail;
  if not AddStrLine(ptHd,Pad(CFG.Miasto,PrnWidth-13)+'Strona:@P') then Fail;
  if not AddStrLine(ptHd,Center('RAPORT  KASOWY  NR  '+long2str(Rg.nr)+
                                '  ZA  OKRES od '+
                                Date2Str(Rg.D1)+' do '+
                                Date2Str(Rg.D2),PrnWidth)) then Fail;
  if not AddStrLine(ptHd,'@R'+char(PrnWidth)+'=') then Fail;
  if not AddStrLine(ptHd,hds) then Fail;
  if not AddStrLine(ptHd,'@R'+char(PrnWidth)+'-') then Fail;
  if not AddStrLine(ptHD,LeftPad('z przeniesienia'+Sep,
                                  PrnWidth-2*ForsaSLen-2*Length(SEP)-KontoLen-LPLen-1)+
                                  '@I0'+sep+'@I1') then Fail;
  if not AddStrLine(ptFt,'@R'+char(PrnWidth)+'-') then Fail;
  if not AddStrLine(ptFt,LeftPad('do przeniesienia'+Sep,
                                  PrnWidth-2*ForsaSLen-2*Length(SEP)-KontoLen-LPLen-1)+
                                  '@I0'+sep+'@I1') then Fail;
  if not AddStrLine(ptEOM,CharSTr('=',PrnWidth)) then Fail;
  if not AddStrLine(ptEOM,LeftPad('Obroty za okres razem'+Sep,
                                  PrnWidth-2*ForsaSLen-2*Length(SEP)-KontoLen-LPLen-1)+
                                  '@I2'+sep+'@I3') then Fail;
  if not AddStrLine(ptEOM,LeftPad('Stany kasy - poprzedni'+Sep,
                                  PrnWidth-2*ForsaSLen-2*Length(SEP)-KontoLen-LPLen-1)+
                                  '@I4'+sep+'@I5') then Fail;
  if not AddStrLine(ptEOM,LeftPad('           - obecny   '+Sep,
                                  PrnWidth-2*ForsaSLen-2*Length(SEP)-KontoLen-LPLen-1)+
                                  '@I6'+sep+'@I7') then Fail;
  if not AddStrLine(ptEOM,LeftPad('liczba zaíÜcznik¢w'+Sep,
                                  PrnWidth-2*ForsaSLen-2*Length(SEP)-KontoLen-LPLen-1)+
                                  'KP @I8'+sep+'KW @I9') then Fail;
  if not SetIntSum(0,0) then Fail; { przeniesienie + }
  if not SetIntSum(1,0) then Fail; { przeniesienie - }
  if not SetIntSum(2,0) then Fail; { suma obrot¢w + }
  if not SetIntSum(3,0) then Fail; { suma obrot¢w - }
  if not SetIntSum(4,0) then Fail; { st. poczÜtk. + }
  if not SetIntSum(5,0) then Fail; { st. poczÜtk. - }
  if not SetIntSum(6,0) then Fail; { st. ko§cowy  + }
  if not SetIntSum(7,0) then Fail; { st. ko§cowy  - }
  if not SetIntSum(8,0) then Fail; { KP }
  if not SetIntSum(9,0) then Fail; { KW }
  Cn:=0;
end; { Init }

function RapKasowy.FormIntSum(Index:integer;N:IntNumber):string;
begin
  if Index<8 then
    FormIntSum:=FormForsa(N)
  else
    FormIntSum:=Form(CharSTr('#',ForsaSLen-3),N);
end;

function RapKasowy.FormLine(var D):PChar;
var TR:OprRecord absolute D;
    s,s1,s2:string;
    F:Forsa;
    b:integer;
begin
  FormLine:=nil;
  Printer^.PrnMsg('Drukujë raport. Teraz z dnia '+Date2Str(TR.data)+' ...');
  if Cn=0 then begin
    if TypBS^.GetNaPlus(TR.typ) then
      F:=TR.stan-TR.kwota
    else
      F:=TR.stan+TR.kwota;
    if F<0 then begin
      SetIntSum(4,0); SetIntSum(5,F);
    end
    else begin
      SetIntSum(4,F); SetIntSum(5,0);
    end;
  end;
  Inc(Cn);
  if Length(TR.nazwa1+TR.nazwa2)+1<=KlnLen then
    s1:=TR.nazwa1+' '+TR.nazwa2
  else
    s1:=TR.nazwa1;
  b:=KlnLen-Length(s1)-1;
  if b>=5 then begin
    if Trim(s1)='' then
      s1:=Copy(TR.opis,1,b)
    else
      s1:=Trim(s1)+' '+Copy(TR.opis,1,b);
  end;
  s1:=Pad(s1,KlnLen);
  if TR.nrdrk=0 then
    s2:=''
  else
    s2:=long2str(TR.nrdrk);
  s:=LeftPad(long2str(Cn),LPLen)+Sep+Date2Str(TR.data)+Sep+
     Pad(TR.rozr,OprRozrSLen)+Sep+s1+Sep;
  if TypBS^.GetNaPlus(TR.typ) then begin
    s:=s+FormForsa(TR.kwota)+Sep+CharSTr(' ',ForsaSLen);
    AddIntSum(0,TR.kwota);
    AddIntSum(2,TR.kwota);
    if TR.nrdrk<>0 then
      AddIntSum(8,1);
  end
  else begin
    s:=s+CharSTr(' ',ForsaSLen)+Sep+FormForsa(TR.kwota);
    AddIntSum(1,TR.kwota);
    AddIntSum(3,TR.kwota);
    if TR.nrdrk<>0 then
      AddIntSum(9,1);
  end;
  s:=s+Sep+LeftPad(s2,LPLen)+Sep+TR.konto;
  if TR.stan < 0 then begin
    SetIntSum(6,0);
    SetIntSum(7,TR.stan);
  end
  else begin
    SetIntSum(6,TR.stan);
    SetIntSum(7,0);
  end;
  LineBufPCopy(s);
  FormLine:=LineBuf;
end; { FormLine }

procedure OprKatal.Drukuj;
  var PDrkO:PRapKasowy;
      bs:BaseState;
begin
  PDrkO:=nil;
  New(PDrkO,Init(OprBS));
  if PDrkO=nil then begin
    Message(txtNoHeap);
    Exit;
  end;
  if PDrkO^.EditOk and
     PDrkO^.Printer^.PrintYesNoQn('Drukowaç raport kasowy') then begin
    OprBS^.SaveBase(bs);
    OprBS^.Select(1);
    PDrkO^.Drukuj(MakeDateKey(PDrkO^.Rg.D1),MakeDateKey(PDrkO^.Rg.D2));
    OprBS^.RestoreBase(bs);
  end;
  Dispose(PDrkO,Done);
end; { Drukuj }

function  OprKatal.TagUnTagOne:integer;
var wr:integer;
begin
  wr := inherited TagUnTagOne;
  if TypBS^.GetNaPlus(OPrBS^.MR.typ) then
    TRWn := TRWn + wr*OPrBS^.MR.Kwota
  else
    TRMa := TRMa + wr*OPrBS^.MR.Kwota;
  TagUnTagOne := wr;
end;

procedure OprKatal.TagUnTagAll;
{ tylko odznacza }
begin
  TRWn:=0;
  TRMa:=0;
  inherited TagUnTagAll;
end;

procedure OprKatal.TagUnTagInfo;
var s:string;
begin
  if TRWn>TRMa then
    s:='Do wpíaty: '+FormForsa(TRWn-TRMa);
  if TRMa>TRWn then
    s:='Do wypíaty: '+FormForsa(TRMa-TRWn);
  if TRMa=TRWn then
    s:='Nic do wypíaty ani wpíaty';
  Write24(' ',_N);
  Message(^C'Zaznaczono '+long2str(TagList^.Count)+' operacji'+
          ^M^C'Przych¢d:  '+FormForsa(TRWn)+
          ^M^C'Rozch¢d:   '+FormForsa(TRMa)+
          ^M^C+s);
end;

procedure OprKatal.ExecUserCommand(Number:byte);
var bs:BaseState;
    PER:PESRObject;
begin
  case Number of
    1:KP;
    2:KW;
    3:begin
        OprBS^.Select(OprBS^.NrMainIndex);
        OprBS^.Previus;
        OprBS^.OdZera:=(not OprBS^.Pass);
        OprBS^.Select(OprBS^.NrMainIndex);
        OprBS^.PrzeliczStan;
      end;
    4:begin { zmiana daty }
       PER:=nil; New(PER,Init);
       if PER=nil then begin Message(txtNOHEAP); exit end;
       with PER^ do
        begin
         FastWrite(CharStr(' ',78),24,2,NormalAttr);
         SetPreEditPtr(ESR,nil);
         LastHelpNo:=hlpDPEdit;
         AddStandardField('DATA',ftDate,'Zmie§ bießÜcÜ datë na :',24,28,
                          DateEPic,0,0,nil,CurrDate);
         Execute;
        end;
       Dispose(PER,Done);
      end;
    5:OprBS^.PrzeliczWszystkie;
  end;
end; { OprKatal.ExecUserCommand }

procedure EditOpr;
var PK:POprKatal;
begin
 PK:=nil; New(PK,Init);
 if PK=nil then begin Message(txtNOHEAP); exit end;
 with PK^ do
  begin
   DisableCommand(cmdTen);
   Wybieracz;
  end;
 Dispose(PK,Done);
end; { EditOpr }


end. { of KSOper unit }
