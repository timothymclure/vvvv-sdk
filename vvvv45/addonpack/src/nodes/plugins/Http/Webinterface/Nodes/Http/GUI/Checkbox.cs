using System;
using System.Collections.Generic;
using System.Text;
using VVVV.PluginInterfaces.V1;
using VVVV.Webinterface.Utilities;
using VVVV.Nodes.Http.BaseNodes;
using VVVV.Webinterface.jQuery;

namespace VVVV.Nodes.Http.GUI
{
    class Checkbox : GuiNodeDynamic, IPlugin, IDisposable
    {

        #region field declaration

        private bool FDisposed = false;

        private IValueIn FDefault;
        private IEnumIn FLook;
        private IValueOut FResponse;


        #endregion field declaration


        #region constructor/destructor

        /// <summary>
        /// the nodes constructor
        /// nothing to declare for this node
        /// </summary>
        public Checkbox()
        {
        }

        /// <summary>
        /// Implementing IDisposable's Dispose method.
        /// Do not make this method virtual.
        /// A derived class should not be able to override this method.
        /// </summary>
        /// 

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            // Take yourself off the Finalization queue
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (FDisposed == false)
            {
                if (disposing)
                {
                    // Dispose managed resources.
                }
                // Release unmanaged resources. If disposing is false,
                // only the following code is executed.
                //mWebinterfaceSingelton.DeleteNode(mObserver);
                FHost.Log(TLogType.Message, FPluginInfo.Name.ToString() + "(Http Gui) Node is being deleted");

                // Note that this is not thread safe.
                // Another thread could start disposing the object
                // after the managed resources are disposed,
                // but before the disposed flag is set to true.
                // If thread safety is necessary, it must be
                // implemented by the client.
            }

            FDisposed = true;
        }


        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in WebTypes derived from this class.
        /// </summary>
        ~Checkbox()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion dispose

        #endregion constructor/destructor


        #region Pugin Information

        public static IPluginInfo FPluginInfo;

        public static IPluginInfo PluginInfo
        {
            get
            {
                if (FPluginInfo == null)
                {
                    //fill out nodes info
                    //see: http://www.vvvv.org/tiki-index.php?page=Conventions.NodeAndPinNaming
                    FPluginInfo = new PluginInfo();

                    //the nodes main name: use CamelCaps and no spaces
                    FPluginInfo.Name = "Checkbox";
                    //the nodes category: try to use an existing one
                    FPluginInfo.Category = "";
                    //the nodes version: optional. leave blank if not
                    //needed to distinguish two nodes of the same name and category
                    FPluginInfo.Version = "";

                    //the nodes author: your sign
                    FPluginInfo.Author = "phlegma";
                    //describe the nodes function
                    FPluginInfo.Help = "Checkbox node for the Renderer (HTTP)";
                    //specify a comma separated list of tags that describe the node
                    FPluginInfo.Tags = "";

                    //give credits to thirdparty code used
                    FPluginInfo.Credits = "";
                    //any known problems?
                    FPluginInfo.Bugs = "";
                    //any known usage of the node that may cause troubles?
                    FPluginInfo.Warnings = "";



                    //leave below as is
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);
                    System.Diagnostics.StackFrame sf = st.GetFrame(0);
                    System.Reflection.MethodBase method = sf.GetMethod();
                    FPluginInfo.Namespace = method.DeclaringType.Namespace;
                    FPluginInfo.Class = method.DeclaringType.Name;
                    //leave above as is
                }

                return FPluginInfo;
            }
        }


        #endregion Plugin Information



        #region pin creation

        protected override void OnSetPluginHost()
        {
            // create required pins
            FHost.CreateValueInput("Default", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FDefault);
            FDefault.SetSubType(0, 1, 1, 0, false, true, true);

            FHost.CreateEnumInput("Look", TSliceMode.Single, TPinVisibility.True, out FLook);
            FLook.SetSubType("LookAndFeel");
            FHost.UpdateEnum("LookAndFeel", "HTML", new string[] { "HTML", "Safari", "GreenAndRed" });

            FHost.CreateValueOutput("Response", 1, null, TSliceMode.Dynamic, TPinVisibility.True, out FResponse);
            FResponse.SetSubType(0, 1, 1, 0, false, false, true);



        }

        #endregion pin creation



        #region Main Loop


        protected override void OnEvaluate(int SpreadMax, bool changedSpreadSize, string NodeId, List<string> SliceId, bool ReceivedNewString, List<string> ReceivedString, List<bool> SendToBrowser)
        {


            if (changedSpreadSize || ReceivedNewString || DynamicPinsAreChanged())
            {
                for (int i = 0; i < SpreadMax; i++)
                {

                    // sets the Slicecount of the Response pin
                    FResponse.SliceCount = SpreadMax;

                    //Gets the Received Slice String from the Server
                    string Response = ReceivedString[i];

                    //reads the Default value from the input pin
                    double currentDefaultSlice;
                    FDefault.GetValue(i, out currentDefaultSlice);

                    //if there is no response use the default value as Response
                    if (ReceivedString[i] == null)
                    {
                        Response = currentDefaultSlice.ToString();
                        FResponse.SetValue(i, currentDefaultSlice);
                    }
                    else
                    {
                        FResponse.SetValue(i, Convert.ToInt16(Response));
                    }


                    //Creates an HTML Checkbox 
                    CheckBox tCheckbox = new CheckBox();

                    //if the Response is 1 set the HTML attribute checked to checked
                    if (Response == "1")
                    {
                        tCheckbox.AddAttribute(new HTMLAttribute("checked", "checked"));
                    }

                    //set the HTML Tag to the GUIDataNode
                    SetTag(i, tCheckbox);


                    //Set Polling Messag, which is send to the server if the FSendPin (DynamicGUINode) is Changed
                    string[] tElementSlider;
                    if (currentDefaultSlice == 0)
                    {
                        tElementSlider = new string[2] { "checked", "" };
                    }
                    else
                    {
                        tElementSlider = new string[2] { "checked", "checked" };
                    }
                    CreatePollingMessage(i, SliceId[i], "attr", tElementSlider);
                }


                //Generate the JavaScript an add it to the GUIDataObject
                JavaScriptVariableObject id = new JavaScriptVariableObject("id");
                JavaScriptVariableObject tThis = new JavaScriptVariableObject("this");

                JQueryExpression postToServerTrue = new JQueryExpression();
                postToServerTrue.Post("ToVVVV.xml", new JavaScriptSnippet(String.Format(@"this.id + '=0'")), null, null);

                JQueryExpression postToServerFalse = new JQueryExpression();
                postToServerFalse.Post("ToVVVV.xml", new JavaScriptSnippet(String.Format(@"this.id + '=1'")), null, null);

                JavaScriptCodeBlock IfStatement = new JavaScriptCodeBlock(JQueryExpression.This().Attr("checked", "false"), postToServerTrue);
                JavaScriptCodeBlock ElseStatement = new JavaScriptCodeBlock(new JQueryExpression().Attr("checked","true"), postToServerFalse);
                JavaScriptCodeBlock Block = new JavaScriptCodeBlock(new JavaScriptIfCondition(new JavaScriptCondition<JavaScriptExpression>(tThis.Member("checked"), "==", true), IfStatement, ElseStatement));

                JavaScriptAnonymousFunction Function = new JavaScriptAnonymousFunction(Block, new string[] { "event" });
                JQueryExpression DocumentReadyHandler = new JQueryExpression(new ClassSelector(GetNodeId(0)));
                DocumentReadyHandler.ApplyMethodCall("click", Function);
                JQuery DocumentReady = JQuery.GenerateDocumentReady(DocumentReadyHandler);


                //reads the lookAndFeel Enum Pin
                string LookAndFeel;
                FLook.GetString(0, out LookAndFeel);

                if (LookAndFeel != "HTML")
                {
                    // createas an Second Document ready to initalise the checkbox plugin for different look an feels
                    JQueryExpression DocumentReadyHandlerLook = new JQueryExpression(new BlankSelector(String.Format(@"""input[class*='{0}']""", GetNodeId(0))));
                    JQuery DocumentReadyLook = JQuery.GenerateDocumentReady(DocumentReadyHandlerLook);

                    if (LookAndFeel == "Safari")
                    {
                        //let the checkboxes look like safari checkboxes
                        JavaScriptGenericObject Option = new JavaScriptGenericObject();
                        Option.Set("cls", new JavaScriptValueLiteral<string>("jquery-safari-checkbox"));
                        DocumentReadyHandlerLook.ApplyMethodCall("checkbox", Option);
                    }
                    if (LookAndFeel == "GreenAndRed")
                    {
                        //let the checkboxes look like green and red On/Off buttons
                        DocumentReadyHandlerLook.ApplyMethodCall("checkbox", null);
                    }

                    //generates the Documente Ready Scripte as Text
                    AddJavaScript(0, DocumentReadyLook.GenerateScript(1, true, true) + Environment.NewLine + DocumentReady.GenerateScript(1, true, true), true);
                }
                else
                {
                    //generates the Documente Ready Scripte as Text
                    AddJavaScript(0, DocumentReady.GenerateScript(1, true, true), true);
                }



            }
        }


        #endregion Main Loop


        protected override bool DynamicPinsAreChanged()
        {
            return FDefault.PinIsChanged || FLook.PinIsChanged;
        }
    }
}