using ApiService;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace LogisticsClientsApp.Providers
{
    
    class DataProvider
    {
        
        public async Task<CargoObject> getTestData()
        {
            UserService.UserServiceClient client = new UserService.UserServiceClient(GrpcChannel.ForAddress("http://localhost:5088"));
            try
            {
                var item = await client.GetCargoAsync(new GetOrDeleteCargoRequest { Id = 1 });
                return await Task.FromResult(item);
            }
            catch (RpcException ex) 
            { 
                return await Task.FromResult(new CargoObject());
            }

        }
    }
}
