using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin.IncrementarNroSocio
{
    public class IncrementarNroSocio : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public IncrementarNroSocio(string unsecureConfig, string secureConfig)
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

            try { 
                    Entity entity = (Entity)context.InputParameters["Target"];

                // Se consulta el mayor numero de Socio
                var consultaMayorNumeroSocio = @"<fetch no-lock='true' aggregate='true' >
                                                      <entity name='dao_socio' >
                                                        <attribute name='dao_nrodesocio' alias='Numero_Socio' aggregate='max' />
                                                      </entity>
                                                    </fetch>";


                FetchExpression fetchConsultaPorNumeroSocio = new FetchExpression(consultaMayorNumeroSocio);
                var RetrieveConsultaPorNumeroSocio = service.RetrieveMultiple(fetchConsultaPorNumeroSocio);

                int codigoAsignar = 1;

                // Se valida que haya al menos 1 registro 
                // Se valida que contenga el atributo "Numero Socio"
                // Se obtiene el mayor número de socio y es incrementado en 1
                if (RetrieveConsultaPorNumeroSocio.Entities.Count > 0 && RetrieveConsultaPorNumeroSocio.Entities[0].Contains("Numero_Socio"))
                {
                    codigoAsignar += (int)((AliasedValue)RetrieveConsultaPorNumeroSocio.Entities[0].Attributes["Numero_Socio"]).Value;
                }
               
                // el valor incrementado es asignado al atributo correspondiente de la entidad
                entity.Attributes["dao_nrodesocio"] = codigoAsignar;  
            }
            catch (Exception e)
            {
                throw new InvalidPluginExecutionException(e.Message);
            }
        }
    }
}