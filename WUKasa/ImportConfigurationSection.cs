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
        [ConfigurationProperty("ImportConfigurations", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ImportConfigCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ImportConfigCollection ImportConfigurations
        {
            get
            {
                return (ImportConfigCollection)base["ImportConfigurations"];
            }
        }
    }

    public class ImportConfig : ConfigurationElement
    {
        public ImportConfig() { }

        public ImportConfig(string name, string operationTyp, string description, bool isIncome)
        {
            Name = name;
            OperationTyp = operationTyp;
            Description = description;
            IsIncome = isIncome;
        }

        [ConfigurationProperty("ID", DefaultValue = "", IsRequired = true, IsKey = true)]
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

        [ConfigurationProperty("BankAmount", IsRequired = false, IsKey = false)]
        public decimal BankAmount
        {
            get { return (decimal)this["BankAmount"]; }
            set { this["BankAmount"] = value; }
        }

    }

    public class ImportConfigCollection : ConfigurationElementCollection
    {
        public ImportConfigCollection()
        {
        }

        public ImportConfig this[int index]
        {
            get { return (ImportConfig)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public void Add(ImportConfig ImportConfig)
        {
            BaseAdd(ImportConfig);
        }

        public void Clear()
        {
            BaseClear();
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ImportConfig();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ImportConfig)element).Name;
        }

        public void Remove(ImportConfig ImportConfig)
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
