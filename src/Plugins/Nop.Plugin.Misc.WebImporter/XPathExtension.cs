using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Nop.Plugin.Misc.WebImporter
{
    class CustomContext : System.Xml.Xsl.XsltContext
    {
        private const string ExtensionsNamespaceUri = "http://xpathExtensions";
        // XsltArgumentList to store names and values of user-defined variables. 
        private XsltArgumentList argList;

        public CustomContext()
        {
        }

        public CustomContext(NameTable nt, XsltArgumentList args)
            : base(nt)
        {
            argList = args;
        }

        // Function to resolve references to user-defined XPath extension  
        // functions in XPath query expressions evaluated by using an  
        // instance of this class as the XsltContext.  
        public override System.Xml.Xsl.IXsltContextFunction ResolveFunction(
                                    string prefix, string name,
                                    System.Xml.XPath.XPathResultType[] argTypes)
        {
            // Verify namespace of function. 
            if (this.LookupNamespace(prefix) == ExtensionsNamespaceUri)
            {
                string strCase = name;

                switch (strCase)
                {
                    case "replace":
                        return new XPathExtensionFunctions(3, 3, XPathResultType.String,
                                                                    argTypes, "replace");
                    case "replaceMore":
                        return new XPathExtensionFunctions(3, 3, XPathResultType.NodeSet,
                                                                    argTypes, "replaceMore");
                }
            }
            // Return null if none of the functions match name. 
            return null;
        }

        // Function to resolve references to user-defined XPath  
        // extension variables in XPath query. 
        public override System.Xml.Xsl.IXsltContextVariable ResolveVariable(
                                                            string prefix, string name)
        {
            if (this.LookupNamespace(prefix) == ExtensionsNamespaceUri || !prefix.Equals(string.Empty))
            {
                throw new XPathException(string.Format("Variable '{0}:{1}' is not defined.", prefix, name));
            }

            // Verify name of function is defined. 
            if (name.Equals("replace") || name.Equals("replaceMore"))
            {
                // Create an instance of an XPathExtensionVariable  
                // (custom IXsltContextVariable implementation) object  
                //  by supplying the name of the user-defined variable to resolve.
                XPathExtensionVariable var;
                var = new XPathExtensionVariable(prefix, name);

                // The Evaluate method of the returned object will be used at run time 
                // to resolve the user-defined variable that is referenced in the XPath 
                // query expression.  
                return var;
            }
            return null;
        }

        // Empty implementation, returns false. 
        public override bool PreserveWhitespace(System.Xml.XPath.XPathNavigator node)
        {
            return false;
        }

        // empty implementation, returns 0. 
        public override int CompareDocument(string baseUri, string nextbaseUri)
        {
            return 0;
        }

        public override bool Whitespace
        {
            get
            {
                return true;
            }
        }

        // The XsltArgumentList property is accessed by the Evaluate method of the  
        // XPathExtensionVariable object that the ResolveVariable method returns. It is used  
        // to resolve references to user-defined variables in XPath query expressions.  
        public XsltArgumentList ArgList
        {
            get
            {
                return argList;
            }
        }
    }

    // The interface that resolves and executes a specified user-defined function.  
    public class XPathExtensionFunctions : System.Xml.Xsl.IXsltContextFunction
    {
        // The data types of the arguments passed to XPath extension function. 
        private System.Xml.XPath.XPathResultType[] argTypes;
        // The minimum number of arguments that can be passed to function. 
        private int minArgs;
        // The maximum number of arguments that can be passed to function. 
        private int maxArgs;
        // The data type returned by extension function. 
        private System.Xml.XPath.XPathResultType returnType;
        // The name of the extension function. 
        private string FunctionName;

        // Constructor used in the ResolveFunction method of the custom XsltContext  
        // class to return an instance of IXsltContextFunction at run time. 
        public XPathExtensionFunctions(int minArgs, int maxArgs,
            XPathResultType returnType, XPathResultType[] argTypes, string functionName)
        {
            this.minArgs = minArgs;
            this.maxArgs = maxArgs;
            this.returnType = returnType;
            this.argTypes = argTypes;
            this.FunctionName = functionName;
        }

        // Readonly property methods to access private fields. 
        public System.Xml.XPath.XPathResultType[] ArgTypes
        {
            get
            {
                return argTypes;
            }
        }
        public int Maxargs
        {
            get
            {
                return maxArgs;
            }
        }

        public int Minargs
        {
            get
            {
                return maxArgs;
            }
        }

        public System.Xml.XPath.XPathResultType ReturnType
        {
            get
            {
                return returnType;
            }
        }

        // XPath extension functions. 

        private string replace(string inputString, string pattern, string replacement)
        {
            return System.Text.RegularExpressions.Regex.Replace(inputString, pattern, replacement, System.Text.RegularExpressions.RegexOptions.Singleline);
        }

        private XPathNodeIterator replaceMore(XPathNodeIterator inputStrings, string pattern, string replacement)
        {
            var clone = new CustomXPathNodeIterator(inputStrings);
            foreach (XPathNavigator node in inputStrings)
            {
                clone.MoveNext();
                ((CustomXPathNavigator)(clone.Current)).SetValue(System.Text.RegularExpressions.Regex.Replace(node.Value, pattern, replacement, System.Text.RegularExpressions.RegexOptions.Singleline));
            }
            clone.Reset();
            return clone;
        }

        // Function to execute a specified user-defined XPath extension  
        // function at run time. 
        public object Invoke(System.Xml.Xsl.XsltContext xsltContext,
                        object[] args, System.Xml.XPath.XPathNavigator docContext)
        {
            if (FunctionName == "replace")
                return (Object)replace((string)args[0], (string)args[1], (string)args[2]);
            if (FunctionName == "replaceMore")
                return (Object)replaceMore((XPathNodeIterator)args[0], (string)args[1], (string)args[2]);

            return null;
        }
    }

    // The interface used to resolve references to user-defined variables 
    // in XPath query expressions at run time. An instance of this class  
    // is returned by the overridden ResolveVariable function of the  
    // custom XsltContext class. 
    public class XPathExtensionVariable : IXsltContextVariable
    {
        // Namespace of user-defined variable. 
        private string prefix;
        // The name of the user-defined variable. 
        private string varName;

        // Constructor used in the overridden ResolveVariable function of custom XsltContext. 
        public XPathExtensionVariable(string prefix, string varName)
        {
            this.prefix = prefix;
            this.varName = varName;
        }

        // Function to return the value of the specified user-defined variable. 
        // The GetParam method of the XsltArgumentList property of the active 
        // XsltContext object returns value assigned to the specified variable. 
        public object Evaluate(System.Xml.Xsl.XsltContext xsltContext)
        {
            XsltArgumentList vars = ((CustomContext)xsltContext).ArgList;
            return vars.GetParam(varName, prefix);
        }

        // Determines whether this variable is a local XSLT variable. 
        // Needed only when using a style sheet. 
        public bool IsLocal
        {
            get
            {
                return false;
            }
        }

        // Determines whether this parameter is an XSLT parameter. 
        // Needed only when using a style sheet. 
        public bool IsParam
        {
            get
            {
                return false;
            }
        }

        public System.Xml.XPath.XPathResultType VariableType
        {
            get
            {
                return XPathResultType.Any;
            }
        }
    }

    public class CustomXPathNodeIterator : XPathNodeIterator
    {
        private List<XPathNavigator> list = new List<XPathNavigator>();
        private int position = -1;

        public CustomXPathNodeIterator(XPathNodeIterator iterator)
        {
            foreach (XPathNavigator nav in iterator)
            {
                list.Add(new CustomXPathNavigator(nav));
            }
        }

        public override XPathNodeIterator Clone()
        {
            return new CustomXPathNodeIterator(this);
        }

        public override XPathNavigator Current
        {
            get { return list[position]; }
        }

        public override int CurrentPosition
        {
            get { return position; }
        }

        public override bool MoveNext()
        {
            position++;
            return (list.Count > position);
        }

        public void Reset()
        {
            position = -1;
        }
    }

    public class CustomXPathNavigator : XPathNavigator
    {
        public XPathNavigator OriginalNavigator { set; get; }
        private string _value;

        public CustomXPathNavigator(XPathNavigator navigator)
        {
            this.OriginalNavigator = navigator;
            this._value = navigator.Value;
        }

        public override string BaseURI
        {
            get { return OriginalNavigator.BaseURI; }
        }

        public override XPathNavigator Clone()
        {
            var clone = new CustomXPathNavigator(OriginalNavigator.Clone());
            clone.SetValue(_value);
            return clone;
        }

        public override bool IsEmptyElement
        {
            get { return OriginalNavigator.IsEmptyElement; }
        }

        public override bool IsSamePosition(XPathNavigator other)
        {
            if (other is CustomXPathNavigator)
                return OriginalNavigator.IsSamePosition(((CustomXPathNavigator)other).OriginalNavigator);
            else
                return false;
        }

        public override string LocalName
        {
            get { return OriginalNavigator.LocalName; }
        }

        public override bool MoveTo(XPathNavigator other)
        {
            return false;
        }

        public override bool MoveToFirstAttribute()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToFirstChild()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToId(string id)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToNext()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToNextAttribute()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            throw new NotImplementedException();
        }

        public override bool MoveToParent()
        {
            throw new NotImplementedException();
        }

        public override bool MoveToPrevious()
        {
            throw new NotImplementedException();
        }

        public override string Name
        {
            get { return OriginalNavigator.Name; }
        }

        public override XmlNameTable NameTable
        {
            get { return OriginalNavigator.NameTable; }
        }

        public override string NamespaceURI
        {
            get { return OriginalNavigator.NamespaceURI; }
        }

        public override XPathNodeType NodeType
        {
            get { return OriginalNavigator.NodeType; }
        }

        public override string Prefix
        {
            get { return OriginalNavigator.Prefix; }
        }

        public override string Value
        {
            get { return _value; }
        }

        public override void SetValue(string value)
        {
            this._value = value;
        }
    }
}
