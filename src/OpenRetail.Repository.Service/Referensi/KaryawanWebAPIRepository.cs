/**
 * Copyright (C) 2017 Kamarudin (http://coding4ever.net/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not
 * use this file except in compliance with the License. You may obtain a copy of
 * the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * The latest version of this file can be found at https://github.com/rudi-krsoftware/open-retail
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using log4net;
using RestSharp;
using Newtonsoft.Json;

using OpenRetail.Model;
using OpenRetail.Model.WebAPI;
using OpenRetail.Repository.Api;
 
namespace OpenRetail.Repository.Service
{        
    public class KaryawanWebAPIRepository : IKaryawanRepository
    {
        private string _apiUrl = string.Empty;
        private ILog _log;
		
        public KaryawanWebAPIRepository(string baseUrl, ILog log)
        {
            this._apiUrl = baseUrl + "api/karyawan/";
            this._log = log;
        }

        public Karyawan GetByID(string id)
        {
            Karyawan obj = null;

			try
            {
                var client = new RestClient(_apiUrl);
                var request = new RestRequest(string.Format("get_by_id?id={0}", id), Method.GET);
                var response = client.Execute<OpenRetailWebApiGetResponse<Karyawan>>(request).Data;

                if (response.Results.Count > 0)
                    obj = response.Results[0];
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return obj;
        }

        public IList<Karyawan> GetByName(string name)
        {
            IList<Karyawan> oList = new List<Karyawan>();

			try
            {
                var client = new RestClient(_apiUrl);
                var request = new RestRequest(string.Format("get_by_name?name={0}", name), Method.GET);
                var response = client.Execute<OpenRetailWebApiGetResponse<Karyawan>>(request).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public IList<Karyawan> GetAll()
        {
            IList<Karyawan> oList = new List<Karyawan>();

			try
            {
                var client = new RestClient(_apiUrl);
                var request = new RestRequest("get_all", Method.GET);
                var response = client.Execute<OpenRetailWebApiGetResponse<Karyawan>>(request).Data;

                if (response.Results.Count > 0)
                    oList = response.Results;
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return oList;
        }

        public int Save(Karyawan obj)
        {
            var result = 0;

			try
            {
                var client = new RestClient(_apiUrl);
                var request = new RestRequest("save", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddBody(obj);

                var response = client.Execute(request);
                var responseContent = JsonConvert.DeserializeObject<OpenRetailWebApiPostResponse>(response.Content);

                result = Convert.ToInt32(responseContent.Results);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Update(Karyawan obj)
        {
            var result = 0;

			try
            {
                var client = new RestClient(_apiUrl);
                var request = new RestRequest("update", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddBody(obj);

                var response = client.Execute(request);
                var responseContent = JsonConvert.DeserializeObject<OpenRetailWebApiPostResponse>(response.Content);

                result = Convert.ToInt32(responseContent.Results);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }

        public int Delete(Karyawan obj)
        {
            var result = 0;

			try
            {
                var client = new RestClient(_apiUrl);
                var request = new RestRequest("delete", Method.POST);

                request.RequestFormat = DataFormat.Json;
                request.AddBody(obj);

                var response = client.Execute(request);
                var responseContent = JsonConvert.DeserializeObject<OpenRetailWebApiPostResponse>(response.Content);

                result = Convert.ToInt32(responseContent.Results);
            }
            catch (Exception ex)
            {
                _log.Error("Error:", ex);
            }

            return result;
        }
    }
}     
