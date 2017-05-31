using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml;

namespace WUKasa.Config
{
    public class ImportConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("ImportRules", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ImportRuleCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ImportRuleCollection ImportRules
        {
            get
            {
                return (ImportRuleCollection)base["ImportRules"];
            }
        }
    }

    public class ImportRule : ConfigurationElement
    {
        public ImportRule() { }

        public ImportRule(string name, string operationTyp, string description, bool isIncome)
        {
            Name = name;
            OperationTyp = operationTyp;
            Description = description;
            IsIncome = isIncome;
        }

        [ConfigurationProperty("Name", DefaultValue = "", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
            set { this["Name"] = value; }
        }

        [ConfigurationProperty("BankAccountRegEx", IsRequired = false, IsKey = false)]
        public string BankAccountRegEx
        {
            get { return (string)this["BankAccountRegEx"]; }
            set { this["BankAccountRegEx"] = value; }
        }

        [ConfigurationProperty("BankDescriptionRegEx", IsRequired = false, IsKey = false)]
        public string BankDescriptionRegEx
        {
            get { return (string)this["BankDescriptionRegEx"]; }
            set { this["BankDescriptionRegEx"] = value; }
        }

        [ConfigurationProperty("BankTitleRegEx", IsRequired = false, IsKey = false)]
        public string BankTitleRegEx
        {
            get { return (string)this["BankTitleRegEx"]; }
            set { this["BankTitleRegEx"] = value; }
        }

        [ConfigurationProperty("SenderReceiverRegEx", IsRequired = false, IsKey = false)]
        public string SenderReceiverRegEx
        {
            get { return (string)this["SenderReceiverRegEx"]; }
            set { this["SenderReceiverRegEx"] = value; }
        }

        [ConfigurationProperty("Description", IsRequired = true, IsKey = false)]
        public string Description
        {
            get { return (string)this["Description"]; }
            set { this["Description"] = value; }
        }

        [ConfigurationProperty("OperationTyp", IsRequired = true, IsKey = false)]
        public string OperationTyp
        {
            get { return (string)this["OperationTyp"]; }
            set { this["OperationTyp"] = value; }
        }

        [ConfigurationProperty("IsIncome", IsRequired = true, IsKey = false)]
        public bool IsIncome
        {
            get { return (bool)this["IsIncome"]; }
            set { this["IsIncome"] = value; }
        }


        [ConfigurationProperty("ActionCode", IsRequired = true, IsKey = false)]
        public int ActionCode
        {
            get { return (int)this["ActionCode"]; }
            set { this["ActionCode"] = value; }
        }

        [ConfigurationProperty("BankAmount", IsRequired = false, IsKey = false)]
        public decimal? BankAmount
        {
            get
            {
                if (this["BankAmount"] != null)
                    return (decimal)this["BankAmount"];
                else
                    return null;
            }
            set { this["BankAmount"] = value; }
        }

        [ConfigurationProperty("ExtractDateFromTitle", IsRequired = false, IsKey = false)]
        public bool? ExtractDateFromTitle
        {
            get
            {
                if (this["ExtractDateFromTitle"] != null)
                    return (bool)this["ExtractDateFromTitle"];
                else
                    return null;
            }
            set { this["ExtractDateFromTitle"] = value; }
        }


        [ConfigurationProperty("CreateBalancingOperation", IsRequired = false, IsKey = false, DefaultValue = true)]
        public bool? CreateBalancingOperation
        {
            get
            {
                if (this["CreateBalancingOperation"] != null)
                    return (bool)this["CreateBalancingOperation"];
                else
                    return null;
            }
            set { this["CreateBalancingOperation"] = value; }
        }

    }

    public class ImportRuleCollection : ConfigurationElementCollection
    {
        public ImportRuleCollection()
        {
        }

        public ImportRule this[int index]
        {
            get { return (ImportRule)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(ImportRule ImportConfig)
        {
            BaseAdd(ImportConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ImportRule();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ImportRule)element).Name;
        }

        public void Remove(ImportRule ImportConfig)
        {
            BaseRemove(ImportConfig.Name);
        }

        public void RemoveAt(int index)
        {
            BaseRemoveAt(index);
        }

        public void Remove(string name)
        {
            BaseRemove(name);
        }
    }
}
