﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Tento kód byl generován nástrojem.
//     Verze modulu runtime:4.0.30319.34003
//
//     Změny tohoto souboru mohou způsobit nesprávné chování a budou ztraceny,
//     dojde-li k novému generování kódu.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 
namespace Nop.Plugin.Widgets.Flexibee.Rezervace.Import {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
    public partial class winstrom {
        
        private winstromRezervace[] rezervaceField;
        
        private winstromVersion versionField;
        
        private bool versionFieldSpecified;
        
        private fbBoolean atomicField;
        
        private bool atomicFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("rezervace", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public winstromRezervace[] rezervace {
            get {
                return this.rezervaceField;
            }
            set {
                this.rezervaceField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public winstromVersion version {
            get {
                return this.versionField;
            }
            set {
                this.versionField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool versionSpecified {
            get {
                return this.versionFieldSpecified;
            }
            set {
                this.versionFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fbBoolean atomic {
            get {
                return this.atomicField;
            }
            set {
                this.atomicField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool atomicSpecified {
            get {
                return this.atomicFieldSpecified;
            }
            set {
                this.atomicFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class winstromRezervace {
        
        private IdType[] idField;
        
        private DatumOdType datumOdField;
        
        private DatumDoType datumDoField;
        
        private MnozstviType mnozstviField;
        
        private PoznamkaType poznamkaField;
        
        private FirmaType firmaField;
        
        private CenikType cenikField;
        
        private SkladType skladField;
        
        private PolObchType polObchField;
        
        private winstromRezervaceUzivatelskevazby uzivatelskevazbyField;
        
        private fbImportMode createField;
        
        private bool createFieldSpecified;
        
        private fbImportMode updateField;
        
        private bool updateFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("id", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public IdType[] id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatumOdType datumOd {
            get {
                return this.datumOdField;
            }
            set {
                this.datumOdField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DatumDoType datumDo {
            get {
                return this.datumDoField;
            }
            set {
                this.datumDoField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public MnozstviType mnozstvi {
            get {
                return this.mnozstviField;
            }
            set {
                this.mnozstviField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public PoznamkaType poznamka {
            get {
                return this.poznamkaField;
            }
            set {
                this.poznamkaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public FirmaType firma {
            get {
                return this.firmaField;
            }
            set {
                this.firmaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public CenikType cenik {
            get {
                return this.cenikField;
            }
            set {
                this.cenikField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public SkladType sklad {
            get {
                return this.skladField;
            }
            set {
                this.skladField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public PolObchType polObch {
            get {
                return this.polObchField;
            }
            set {
                this.polObchField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("uzivatelske-vazby", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public winstromRezervaceUzivatelskevazby uzivatelskevazby {
            get {
                return this.uzivatelskevazbyField;
            }
            set {
                this.uzivatelskevazbyField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fbImportMode create {
            get {
                return this.createField;
            }
            set {
                this.createField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool createSpecified {
            get {
                return this.createFieldSpecified;
            }
            set {
                this.createFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fbImportMode update {
            get {
                return this.updateField;
            }
            set {
                this.updateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool updateSpecified {
            get {
                return this.updateFieldSpecified;
            }
            set {
                this.updateFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class IdType {
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="Uzivatelske-vazbyVazbaTypType")]
    public partial class UzivatelskevazbyVazbaTypType {
        
        private fbIfNotFoundMode ifnotfoundField;
        
        private bool ifnotfoundFieldSpecified;
        
        private string previousValueField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("if-not-found")]
        public fbIfNotFoundMode ifnotfound {
            get {
                return this.ifnotfoundField;
            }
            set {
                this.ifnotfoundField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ifnotfoundSpecified {
            get {
                return this.ifnotfoundFieldSpecified;
            }
            set {
                this.ifnotfoundFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    public enum fbIfNotFoundMode {
        
        /// <remarks/>
        @null,
        
        /// <remarks/>
        fail,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="Uzivatelske-vazbyPoznamType")]
    public partial class UzivatelskevazbyPoznamType {
        
        private string previousValueField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="Uzivatelske-vazbyPopisType")]
    public partial class UzivatelskevazbyPopisType {
        
        private string previousValueField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(TypeName="Uzivatelske-vazbyIdType")]
    public partial class UzivatelskevazbyIdType {
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PolObchType {
        
        private fbIfNotFoundMode ifnotfoundField;
        
        private bool ifnotfoundFieldSpecified;
        
        private string previousValueField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("if-not-found")]
        public fbIfNotFoundMode ifnotfound {
            get {
                return this.ifnotfoundField;
            }
            set {
                this.ifnotfoundField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ifnotfoundSpecified {
            get {
                return this.ifnotfoundFieldSpecified;
            }
            set {
                this.ifnotfoundFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SkladType {
        
        private fbIfNotFoundMode ifnotfoundField;
        
        private bool ifnotfoundFieldSpecified;
        
        private string previousValueField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("if-not-found")]
        public fbIfNotFoundMode ifnotfound {
            get {
                return this.ifnotfoundField;
            }
            set {
                this.ifnotfoundField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ifnotfoundSpecified {
            get {
                return this.ifnotfoundFieldSpecified;
            }
            set {
                this.ifnotfoundFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class CenikType {
        
        private fbIfNotFoundMode ifnotfoundField;
        
        private bool ifnotfoundFieldSpecified;
        
        private string previousValueField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("if-not-found")]
        public fbIfNotFoundMode ifnotfound {
            get {
                return this.ifnotfoundField;
            }
            set {
                this.ifnotfoundField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ifnotfoundSpecified {
            get {
                return this.ifnotfoundFieldSpecified;
            }
            set {
                this.ifnotfoundFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class FirmaType {
        
        private fbIfNotFoundMode ifnotfoundField;
        
        private bool ifnotfoundFieldSpecified;
        
        private string previousValueField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("if-not-found")]
        public fbIfNotFoundMode ifnotfound {
            get {
                return this.ifnotfoundField;
            }
            set {
                this.ifnotfoundField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool ifnotfoundSpecified {
            get {
                return this.ifnotfoundFieldSpecified;
            }
            set {
                this.ifnotfoundFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class PoznamkaType {
        
        private string previousValueField;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class MnozstviType {
        
        private decimal previousValueField;
        
        private bool previousValueFieldSpecified;
        
        private decimal valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public decimal previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool previousValueSpecified {
            get {
                return this.previousValueFieldSpecified;
            }
            set {
                this.previousValueFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public decimal Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DatumDoType {
        
        private System.DateTime previousValueField;
        
        private bool previousValueFieldSpecified;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date")]
        public System.DateTime previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool previousValueSpecified {
            get {
                return this.previousValueFieldSpecified;
            }
            set {
                this.previousValueFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class DatumOdType {
        
        private System.DateTime previousValueField;
        
        private bool previousValueFieldSpecified;
        
        private System.DateTime valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public System.DateTime previousValue {
            get {
                return this.previousValueField;
            }
            set {
                this.previousValueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool previousValueSpecified {
            get {
                return this.previousValueFieldSpecified;
            }
            set {
                this.previousValueFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public System.DateTime Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class winstromRezervaceUzivatelskevazby {
        
        private winstromRezervaceUzivatelskevazbyUzivatelskavazba[] uzivatelskavazbaField;
        
        private fbBoolean removeAllField;
        
        private bool removeAllFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("uzivatelska-vazba", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public winstromRezervaceUzivatelskevazbyUzivatelskavazba[] uzivatelskavazba {
            get {
                return this.uzivatelskavazbaField;
            }
            set {
                this.uzivatelskavazbaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fbBoolean removeAll {
            get {
                return this.removeAllField;
            }
            set {
                this.removeAllField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool removeAllSpecified {
            get {
                return this.removeAllFieldSpecified;
            }
            set {
                this.removeAllFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public partial class winstromRezervaceUzivatelskevazbyUzivatelskavazba {
        
        private UzivatelskevazbyIdType[] idField;
        
        private UzivatelskevazbyPopisType popisField;
        
        private UzivatelskevazbyPoznamType poznamField;
        
        private UzivatelskevazbyVazbaTypType vazbaTypField;
        
        private fbImportMode createField;
        
        private bool createFieldSpecified;
        
        private fbImportMode updateField;
        
        private bool updateFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("id", Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public UzivatelskevazbyIdType[] id {
            get {
                return this.idField;
            }
            set {
                this.idField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public UzivatelskevazbyPopisType popis {
            get {
                return this.popisField;
            }
            set {
                this.popisField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public UzivatelskevazbyPoznamType poznam {
            get {
                return this.poznamField;
            }
            set {
                this.poznamField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public UzivatelskevazbyVazbaTypType vazbaTyp {
            get {
                return this.vazbaTypField;
            }
            set {
                this.vazbaTypField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fbImportMode create {
            get {
                return this.createField;
            }
            set {
                this.createField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool createSpecified {
            get {
                return this.createFieldSpecified;
            }
            set {
                this.createFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public fbImportMode update {
            get {
                return this.updateField;
            }
            set {
                this.updateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool updateSpecified {
            get {
                return this.updateFieldSpecified;
            }
            set {
                this.updateFieldSpecified = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    public enum fbImportMode {
        
        /// <remarks/>
        ok,
        
        /// <remarks/>
        ignore,
        
        /// <remarks/>
        fail,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    public enum fbBoolean {
        
        /// <remarks/>
        @true,
        
        /// <remarks/>
        @false,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
    public enum winstromVersion {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1.0")]
        Item10,
    }
}
