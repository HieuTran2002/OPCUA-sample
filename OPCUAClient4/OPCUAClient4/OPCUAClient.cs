using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Globalization;

namespace OPCUAClient
{
    public class OPCUAClient
    {
        #region private

        /// <summary>
        /// create client configuration
        /// </summary>
        /// <returns></returns>
        private Session mSession;
        private Subscription mSubscription;
        private static ApplicationConfiguration CreateClientConfiguration()
        {
            //new application configura
            ApplicationConfiguration config = new ApplicationConfiguration()
            {
                ApplicationName = "TestClient",
                ApplicationUri = Utils.Format(@"urn:{0}:TestClient", System.Net.Dns.GetHostName()),
                ApplicationType = ApplicationType.Client,
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier { StoreType = @"Directory", StorePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"/Cert/CertificateIdentifier", SubjectName = "Client" },
                    TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"/Cert/CertificateTrustList" },
                    TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"/Cert/CertificateTrustList" },
                    RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + @"/Cert/CertificateTrustList" },
                    AutoAcceptUntrustedCertificates = true,
                    RejectSHA1SignedCertificates = false
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
                TraceConfiguration = new TraceConfiguration()

            };
            //validate configuration
            config.Validate(ApplicationType.Client);
            return config;
        }
        private static EndpointDescription CreateEndpointDescription(string url, string secPolicy, MessageSecurityMode msgSecMode)
        {
            EndpointDescription endpointDescription = new EndpointDescription();

            endpointDescription.EndpointUrl = url;
            endpointDescription.SecurityPolicyUri = secPolicy;
            endpointDescription.SecurityMode = msgSecMode;
            endpointDescription.TransportProfileUri = Profiles.UaTcpTransport;

            return endpointDescription;
        }


        #endregion

        #region public 
        public ApplicationConfiguration applicationConfiguration;
        public CertificateValidationEventHandler CertificateValidationNotification = null;
        public EndpointConfiguration endpointConfiguration;
        public EndpointDescription endpointDescription;
        public ConfiguredEndpoint configuredEndpoint;
        public MonitoredItemNotificationEventHandler itemChangedNotification = null;


        #region Accessors
        public Session Session
        {
            get { return mSession; }
        }

        #endregion
        public OPCUAClient()
        {
            //get new application setting
            applicationConfiguration = CreateClientConfiguration();
        }
        #endregion

        #region Connect / disconnect

        public async Task Connect(string url, string secPolicy, MessageSecurityMode msgSecMode, bool userAuth, string userName, string password)
        {
            //MessageBox.Show("start connect");

            await Task.Run(() =>
            {
                try
                {
                    //Hook up a validator function for a CertificateValidation event
                    applicationConfiguration.CertificateValidator.CertificateValidation += Notificatio_CertificateValidation;

                    // the old code 
                    //endpointDescription = CreateEndpointDescription(url, secPolicy, msgSecMode);

                    //Try some other way to create endpoint desciption, the code above trigger some exception
                    endpointDescription = CoreClientUtils.SelectEndpoint(url, userAuth);

                    //Create EndPoint configuration
                    endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);

                    //Create an Endpoint object to connect to server
                    ConfiguredEndpoint Endpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                    //Create new user identity
                    UserIdentity UserIdentity;
                    if (userAuth)
                    {
                        //sign in with username and password
                        UserIdentity = new UserIdentity(userName, password);
                    }
                    else
                    {
                        //sign in as anonymous
                        UserIdentity = new UserIdentity();
                    }

                    //Update certificate store before connection attempt
                    applicationConfiguration.CertificateValidator.Update(applicationConfiguration);

                    //Create and connect session
                    mSession = Session.Create(
                         applicationConfiguration,
                         Endpoint,
                         true,
                         "MySession",
                         60000,
                         UserIdentity,
                         null
                         ).GetAwaiter().GetResult();

                    mSession.KeepAlive += new KeepAliveEventHandler(Notification_KeepAlive);
                }
                catch (Exception e)
                {
                    //handle Exception here
                    throw e;
                }
            });
        }

        /// <summary>
        /// Optimize the connection
        /// </summary>
        /// <param name="endpointDescription"The EndpointDescription of server's enpoint </param>
        /// <param name="userAuth"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        public void Connect(EndpointDescription endpointDescription, bool userAuth, string userName, string password)
        {
            try
            {

                applicationConfiguration.CertificateValidator.CertificateValidation += Notificatio_CertificateValidation;

                endpointConfiguration = EndpointConfiguration.Create(applicationConfiguration);

                configuredEndpoint = new ConfiguredEndpoint(null, endpointDescription, endpointConfiguration);

                String sessionName = "MySession";

                //specity user identity
                UserIdentity UserIdentity;
                if (userAuth)
                {
                    UserIdentity = new UserIdentity(userName, password);
                }
                else
                {
                    UserIdentity = new UserIdentity();
                }

                //update application configuration before connect
                applicationConfiguration.CertificateValidator.Update(applicationConfiguration);

                //create session and connect
                mSession = Session.Create(
                    applicationConfiguration,
                    configuredEndpoint,
                    true,
                    sessionName,
                    60000,
                    UserIdentity,
                    null
                    ).GetAwaiter().GetResult();


                mSession.KeepAlive += new KeepAliveEventHandler(Notification_KeepAlive);
            }
            catch (Exception e)
            {
                throw e;
            }
        }


        private void Notification_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            ;
        }

        /// <summary>
        /// Disconnect from server
        /// </summary>
        public void Disconnect()
        {
            try
            {
                //dispose session
                mSession.Close(5000);
                mSession.Dispose();

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        #endregion

        #region browser

        /// <summary>
        /// Get root folder, then search for all item in namespace
        /// </summary>
        /// <param name="references"></param>
        /// <param name="node"></param>
        public void BrowseNodes(ReferenceDescription references, TreeNode node)
        {
            try
            {
                ReferenceDescriptionCollection nextReferenceDescriptions;
                byte[] continuationPoint;

                NodeId nodeId = ExpandedNodeId.ToNodeId(references.NodeId, null);

                mSession.Browse(null, null, nodeId, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out continuationPoint, out nextReferenceDescriptions);
                foreach (var rd in nextReferenceDescriptions)
                {
                    int idx = GetNodeClaseIdx(rd);

                    TreeNode newNode = new TreeNode() { Text = rd.DisplayName.ToString(), Tag = rd, ImageIndex = idx, SelectedImageIndex = idx };
                    node.Nodes.Add(newNode);
                    BrowseNodes(rd, newNode);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.TargetSite.Name);
            }
        }

        public ReferenceDescriptionCollection browseNodes(ReferenceDescription reference)
        {
            ReferenceDescriptionCollection referenceDescriptionsCollection;
            ReferenceDescriptionCollection nextReferenceDescriptionsCollection;
            byte[] continuationPoint;
            byte[] revisedContinuationPoint;
            NodeId nodeid = ExpandedNodeId.ToNodeId(reference.NodeId, null);
            
            try
            {
            
                mSession.Browse(null, null, nodeid, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, 0, out continuationPoint, out referenceDescriptionsCollection);

                while (continuationPoint != null)
                {
                    mSession.BrowseNext(null, false, continuationPoint, out revisedContinuationPoint, out nextReferenceDescriptionsCollection);
                    referenceDescriptionsCollection.AddRange(nextReferenceDescriptionsCollection);
                    continuationPoint = revisedContinuationPoint;
                }

                return referenceDescriptionsCollection;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        /// <summary>
        /// Search for all folder in the first layer of namespace
        /// </summary>
        /// <returns></returns>
        public ReferenceDescriptionCollection BrowseObjectFolders()
        {
            try
            {
                ReferenceDescriptionCollection referenceDescriptions = new ReferenceDescriptionCollection();
                byte[] continuationPoint;
                mSession.Browse(null, null, ObjectIds.RootFolder, 0u, BrowseDirection.Forward, ReferenceTypeIds.HierarchicalReferences, true, (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method, out continuationPoint, out referenceDescriptions);
                return referenceDescriptions;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

         public int GetNodeClaseIdx(ReferenceDescription rd)
        {
            int idx = 0;
            switch (rd.NodeClass.ToString())
            {
                case "Object":
                    idx = 0;
                    break;
                case "Variable":
                    idx = 1;
                    break;
                case "Method":
                    idx = 2;
                    break;
                case "View":
                    idx = 3;
                    break;
                case "ObjectType":
                    idx = 4;
                    break;
                case "VariableType":
                    idx = 5;
                    break;
                case "ReferenceType":
                    idx = 6;
                    break;
                case "DataType":
                    idx = 7;
                    break;

            }
            return idx;
        }

        #endregion

        #region read/write

        public Node readNode(String nodeID)
        {
            NodeId nodeId = new NodeId(nodeID);
            Node node = new Node();
            try
            {
                node = mSession.ReadNode(nodeId);

                return node;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<string> readValues(List<string> nodeIdString)
        {
            List<NodeId> nodeIds = new List<NodeId>();
            List<Type> types = new List<Type>();
            List<object> values = new List<object>();
            List<ServiceResult> serviceResults = new List<ServiceResult>();


            foreach (string str in nodeIdString)
            {
                nodeIds.Add(new NodeId(str));
                types.Add(null);
            }
            try
            {
                mSession.ReadValues(nodeIds, types, out values, out serviceResults);
                foreach (ServiceResult svResult in serviceResults)
                {
                    if (svResult.ToString() != "Good")
                    {
                        Exception e = new Exception(svResult.ToString());
                        throw e;
                    }
                }
                List<string> resultStrings = new List<string>();
                foreach (object result in values)
                {
                    resultStrings.Add(result.ToString());
                }

                return resultStrings;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
        public string readDataType(string nodeId)
        {
            string returnData = "";
            ReadValueIdCollection readValues = new ReadValueIdCollection()
            {
                new ReadValueId() {NodeId = nodeId, AttributeId = Attributes.DataType }
            };

            Session.Read(null, 0, TimestampsToReturn.Both, readValues, out DataValueCollection values, out DiagnosticInfoCollection diagnosticInfos);
            foreach (DataValue datatype in values)
            {
                returnData = datatype.ToString();
                break;
            }
            return returnData;


        }

        public void writeValues(List<string> values, List<string> nodeIdStrings, List<string> dataTypes)
        {
            WriteValueCollection writeValues = new WriteValueCollection();

            StatusCodeCollection statusCodes = new StatusCodeCollection();

            DiagnosticInfoCollection diagnosticInfos = new DiagnosticInfoCollection();

            foreach (string str in nodeIdStrings)
            {
                NodeId nodeId = new NodeId(str);

                //string test = dataValue.Value.GetType().Name;

                Variant variant = 0;
                try
                {
                    variant = CreateVariantToWrite(values[nodeIdStrings.IndexOf(str)], dataTypes[nodeIdStrings.IndexOf(str)]);

                }
                catch (Exception e)
                {
                    throw e;

                }


                WriteValue valueToWrite = new WriteValue();

                valueToWrite.Value = new DataValue(variant);
                valueToWrite.NodeId = nodeId;
                valueToWrite.AttributeId = Attributes.Value;

                writeValues.Add(valueToWrite);

            }

            try
            {

                mSession.Write(null, writeValues, out statusCodes, out diagnosticInfos);
                foreach (StatusCode code in statusCodes)
                {
                    if (code != 0)
                    {
                        Exception ex = new Exception(code.ToString());
                        throw ex;
                    }

                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public Variant CreateVariantToWrite(string value, string datatype)
        {
            switch (datatype)
            {
                case "Boolean":
                    return new Variant(Convert.ToBoolean(value));
                case "Double":
                    return new Variant(Convert.ToDouble(value));
                case "Float":
                    return new Variant(float.Parse(value, CultureInfo.InvariantCulture.NumberFormat));
                case "SByte":
                    return new Variant(Convert.ToSByte(value));
                case "Byte":
                    return new Variant(Convert.ToByte(value));
                case "Int16":
                    return new Variant(Convert.ToInt16(value));
                case "UInt16":
                    return new Variant(Convert.ToUInt16(value));
                case "Int32":
                    return new Variant(Convert.ToInt32(value));
                case "UInt32":
                    return new Variant(Convert.ToUInt32(value));
                case "Int64":
                    return new Variant(Convert.ToInt64(value));
                case "UInt64":
                    return new Variant(Convert.ToUInt64(value));
                case "String":
                    return new Variant(value);

                default:
                    return new Variant();
            }
        }
        #endregion

        #region subcription 
        public Subscription subscribe(int publishingInterval)
        {

            Subscription subscription = new Subscription(mSession.DefaultSubscription);

            subscription.PublishingEnabled = true;
            subscription.PublishingInterval = publishingInterval;
            mSession.AddSubscription(subscription);
            try
            {
                subscription.Create();
                return subscription;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Opc.Ua.Client.MonitoredItem addMonitoredItem(Subscription subscription, string nodeID, string itemName, int samplingInterval)
        {
            //new moniteredItem
            Opc.Ua.Client.MonitoredItem monitoredItem = new Opc.Ua.Client.MonitoredItem();

            //Set properties
            monitoredItem.DisplayName = itemName;
            monitoredItem.StartNodeId = nodeID;
            monitoredItem.AttributeId = Attributes.Value;



            monitoredItem.MonitoringMode = MonitoringMode.Reporting;
            monitoredItem.QueueSize = 1;
            monitoredItem.DiscardOldest = true;
            monitoredItem.Notification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);

            try
            {
                subscription.AddItem(monitoredItem);
                subscription.ApplyChanges();
                return monitoredItem;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Opc.Ua.Client.MonitoredItem addMonitoredItemDataType(Subscription subscription, string nodeID, string itemName, int samplingInterval)
        {
            //new moniteredItem
            Opc.Ua.Client.MonitoredItem monitoredItem = new Opc.Ua.Client.MonitoredItem();

            //Set properties
            monitoredItem.AttributeId = Attributes.DataType;

            monitoredItem.QueueSize = 1;
            monitoredItem.DiscardOldest = true;
            monitoredItem.Notification += new MonitoredItemNotificationEventHandler(Notification_MonitoredItem);

            try
            {
                subscription.AddItem(monitoredItem);
                subscription.ApplyChanges();
                return monitoredItem;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public Opc.Ua.Client.MonitoredItem addEventMonitoredItem(Subscription subscription, string nodeID, string itemName, int samplingInterval, EventFilter filter)
        {
            Opc.Ua.Client.MonitoredItem monitoredItem = new Opc.Ua.Client.MonitoredItem(subscription.DefaultItem);

            monitoredItem.DisplayName = itemName;
            monitoredItem.StartNodeId = nodeID;
            monitoredItem.AttributeId = Attributes.EventNotifier;
            monitoredItem.MonitoringMode = MonitoringMode.Reporting;
            monitoredItem.SamplingInterval = samplingInterval;
            monitoredItem.QueueSize = 1;
            monitoredItem.DiscardOldest = true;
            monitoredItem.Filter = filter;

            mSession.Notification += new NotificationEventHandler(Notification_MonitoredEventItem);

            try
            {
                subscription.AddItem(monitoredItem);
                subscription.ApplyChanges();
                return monitoredItem;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Opc.Ua.Client.MonitoredItem removeMonitoredItem(Subscription subscription, Opc.Ua.Client.MonitoredItem monitoredItem)
        {
            try
            {
                subscription.RemoveItem(monitoredItem);
                subscription.ApplyChanges();
                return monitoredItem;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void removeSubsciption(Subscription subscription)
        {
            try
            {
                subscription.Delete(true);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        #endregion

        #region event handling

        private void Notification_MonitoredEventItem(Session session, NotificationEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Notification_MonitoredItem(Opc.Ua.Client.MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs e)
        {
            itemChangedNotification(monitoredItem, e);
        }

        private void Notificatio_CertificateValidation(CertificateValidator certificate, CertificateValidationEventArgs e)
        {
            //CertificateValidationNotification(certificate, e);
        }
        #endregion

    }
}
