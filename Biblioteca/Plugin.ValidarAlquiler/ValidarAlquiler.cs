using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin.ValidarAlquiler
{
    public class ValidarAlquiler : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public ValidarAlquiler(string unsecureConfig, string secureConfig)
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

            if (entity.LogicalName.Equals("dao_alquiler"));
            {
                Guid idSocioEntidad = ((EntityReference)entity.Attributes["dao_socioid"]).Id;
                Guid idBibliotecaEntidad = ((EntityReference)entity.Attributes["dao_bibliotecaid"]).Id;
                Guid idLibroEntidad = ((EntityReference)entity.Attributes["dao_libroid"]).Id;

                ////Query Expression
                //  Validar la existencia de la relacion Socio - Libro

                var ConsultaSocioLibro = new QueryExpression("dao_alquiler");
                ConsultaSocioLibro.TopCount = 1;
                ConsultaSocioLibro.NoLock = true;
                ConsultaSocioLibro.ColumnSet.AddColumns("dao_socioid", "dao_libroid");
                ConsultaSocioLibro.Criteria = new FilterExpression();
                ConsultaSocioLibro.Criteria.AddCondition("dao_socioid", ConditionOperator.Equal, idSocioEntidad);
                ConsultaSocioLibro.Criteria.AddCondition("dao_libroid", ConditionOperator.Equal, idLibroEntidad);

                EntityCollection RetrieveConsultaSocioLibro = service.RetrieveMultiple(ConsultaSocioLibro);

                if (RetrieveConsultaSocioLibro.Entities.Count ==1)
                {
                    throw new InvalidPluginExecutionException("*** El socio ya tiene en alquier este libro ***");
                }

                //validar unidades disponibles en la entidad BibliotecaLibro

                String consultaStockDisponible = @"<fetch top='1' no-lock='true' >
                                                  <entity name='dao_bibliotecalibro' >
                                                    <attribute name='dao_unidades' />
                                                    <filter type='and' >
                                                      <condition attribute='dao_bibliotecaid' operator='eq' value='" + idLibroEntidad + @"' />
                                                      <condition attribute='dao_libroid' operator='eq' value='" + idBibliotecaEntidad  + @"' />
                                                    </filter>
                                                  </entity>
                                                </fetch>";

                FetchExpression stockDisponible = new FetchExpression(consultaStockDisponible);
                var resultadoStockDisponible = service.RetrieveMultiple(stockDisponible);

                var val = resultadoStockDisponible.Entities;

                if ((int)val.Count == 0 || val == null)
                {
                    throw new InvalidPluginExecutionException("*** No existe la relación entre el Libro y la Biblioteca seleccionados ***");

                }else if ((int)val[0].Attributes["dao_unidades"] <= 0)
                {
                    throw new InvalidPluginExecutionException("*** No hay unidades disponibles ***");
                }
            }
        }
    }
}

