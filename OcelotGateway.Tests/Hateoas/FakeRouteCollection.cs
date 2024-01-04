using OcelotGateway.Hateoas;
using OcelotGateway.Hateoas.Configurations;
using System.Security;

namespace OcelotGateway.Tests.Hateoas;

internal class FakeRouteCollection : IRouteCollection
{
    public bool TryGetRoute(string routeName, out Route route)
    {
        route = null;
        switch (routeName)
        {
            case RoutesNames.GetContracts:
                route = new Route(RoutesNames.GetContracts,
                    new LinkTemplate("/get-contracts"),
                    "GET", new List<Permission>());
                return true;
            case RoutesNames.GetContract:
                route = new Route(RoutesNames.GetContract,
                    new LinkTemplate("/get-contract/{contractId}"),
                    "GET", new List<Permission>());
                return true;

            case RoutesNames.GetPersonalDataByContract:
                route = new Route(RoutesNames.GetPersonalDataByContract,
                    new LinkTemplate(
                        "/get-personal-data-by-contract/{contractId}?page={page}&pageSize={pageSize}&sortBy={sortBy}&sortOrder={sortOrder}&filterBy={filterBy}&filterCriteria={filterCriteria}"),
                    "GET", new List<Permission>());
                return true;
        }

        return false;
    }
}