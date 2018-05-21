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

                var statecode = entity.Attributes["statecode"];
                var statuscode = entity.Attributes["statuscode"];

                //TODO: Do stuff

                /////// Linea de prueba
  
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}