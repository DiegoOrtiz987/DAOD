using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin.ValidacionDni
{
    public class ValidarDniExistenteSocio : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public ValidarDniExistenteSocio(string unsecureConfig, string secureConfig)
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

            Entity entity = (Entity)context.InputParameters["Target"];

            //// Validar existencia de DNI 

            ////Capturar el DNI que se esta ingresando
            var DniDigitado = entity.Attributes["dao_dni"];

            //// QueryExpression
            // Se consulta la existencia de el DNI ingresado

            var ConsultaPorDni = new QueryExpression("dao_socio");
            ConsultaPorDni.TopCount = 1;
            ConsultaPorDni.NoLock = true;
            ConsultaPorDni.ColumnSet = new ColumnSet("dao_nrodesocio");
            ConsultaPorDni.Criteria = new FilterExpression();
            ConsultaPorDni.Criteria.AddCondition("dao_dni", ConditionOperator.Equal, DniDigitado);

            //
            EntityCollection RetrieveConsultaPorDni = service.RetrieveMultiple(ConsultaPorDni);

            //Se evalua la cantidad de registros encontrados 

            if (RetrieveConsultaPorDni.Entities.Count > 0)
                {
                    throw new InvalidPluginExecutionException ("El DNI digitado ya existe");
                }
  
        }
    }
}