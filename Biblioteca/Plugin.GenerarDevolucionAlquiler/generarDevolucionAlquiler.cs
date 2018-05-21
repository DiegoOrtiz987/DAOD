using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin.GenerarDevolucionAlquiler
{
    public class generarDevolucionAlquiler : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public generarDevolucionAlquiler(string unsecureConfig, string secureConfig)
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecureConfig;
        }
        #endregion
        public void Execute(IServiceProvider serviceProvider)
        {
            ITracingService tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            IOrganizationServiceFactory factory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            IOrganizationService service = factory.CreateOrganizationService(context.UserId);


            try
            {
                Entity entity = (Entity)context.InputParameters["Target"];

                //EntityReference alquiler = (EntityReference)context.InputParameters["Alquiler"];

                //bool devolverOk = (bool)context.OutputParameters["DevolucionOk"];


                //OrganizationRequest request = new OrganizationRequest() { RequestName = "SetState" };

                //int state = ((OptionSetValue)request["SetState"]).Value;

                SetStateRequest setStateReq = new SetStateRequest();
                setStateReq.EntityMoniker = new EntityReference("dao_alquiler",entity.Id);
                setStateReq.State = new OptionSetValue(1);
                setStateReq.Status = new OptionSetValue(2);
                entity.Attributes["statecode"] = setStateReq.State;
                entity.Attributes["statuscode"] = setStateReq.Status;

                service.Update(entity);


                /*SetStateResponse response = (SetStateResponse)_connection.CrmService.Execute(setStateReq);

                /*if (entity.Attributes.Contains("statecode"))
                {
                    //OptionSetValue status = (OptionSetValue)entity.Attributes["statuscode"];
                    int valorState = ((OptionSetValue)entity.Attributes["statecode"]).Value;
                    OptionSetValue statecode = new OptionSetValue {Value = 810700000};
                    entity.Attributes["statecode"] = statecode;
                    service.Update(entity);
                }

                if (entity.Attributes.Contains("statuscode"))
                {
                    //OptionSetValue status = (OptionSetValue)entity.Attributes["statuscode"];
                    int valorStatus = ((OptionSetValue)entity.Attributes["statuscode"]).Value;
                    OptionSetValue status = new OptionSetValue {Value = 810700000};
                    entity.Attributes["statuscode"] = status;
                    
                }*/
                //var statecode = entity.Attributes["statecode"];

               
                //var statecode = entity.Attributes["statecode"];
                //var statuscode = entity.Attributes["statuscode"];

                //TODO: Do stuff



                //TODO: Do stuff


                ///////*/
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}