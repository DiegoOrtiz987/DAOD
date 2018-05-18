using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;

namespace Plugin.ValidarIsbnLibro
{
    public class ValidarIsbnLibro : IPlugin
    {
        #region Secure/Unsecure Configuration Setup
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public ValidarIsbnLibro(string unsecureConfig, string secureConfig)
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

            #region Validación ISBN
            //Se verifica la existencia de la entidad y atributo 
            if (entity.LogicalName.Equals("dao_libro") && entity.Attributes.Contains("dao_isbn"))
                {
                    //Captura del valor digitado en el atributo isbn
                    var isbnDigitado = entity.Attributes["dao_isbn"];

                    //// Query expression 
                    // Consulta si existe el dato capturado en el atributo "isbnDigitado"
                    var ConsultaPorIsbn = new QueryExpression("dao_libro");
                    ConsultaPorIsbn.NoLock = true;
                    ConsultaPorIsbn.TopCount = 1;
                    //ConsultaPorIsbn.ColumnSet = new ColumnSet("dao_isbn");
                    ConsultaPorIsbn.Criteria = new FilterExpression();
                    ConsultaPorIsbn.Criteria.AddCondition("dao_isbn", ConditionOperator.Equal, isbnDigitado);

                    EntityCollection RetrieveConsultaPorIsbn = service.RetrieveMultiple(ConsultaPorIsbn);
                    
                    //Se evalua la cantidad de entidades encontrados
                    if (RetrieveConsultaPorIsbn.Entities.Count > 0)
                    {
                        throw new InvalidPluginExecutionException("El ISBN digitado ya existe");
                    }

                }
                else
                {
                    throw new InvalidPluginExecutionException("No hay existencia de la entidad o atributo");
                }
            #endregion
        }
    }
}