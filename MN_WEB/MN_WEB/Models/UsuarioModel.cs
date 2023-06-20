using MN_WEB.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Web;
using System.Web.Mvc;

namespace MN_WEB.Models
{
    public class UsuarioModel
    {
        public UsuarioEnt IniciarSesion(UsuarioEnt entidad)
        {
            using (var client = new HttpClient())
            {
                string url = "https://localhost:44351/api/IniciarSesion";
                JsonContent body = JsonContent.Create(entidad); //serializar
                HttpResponseMessage resp = client.PostAsync(url, body).Result;

                if(resp.IsSuccessStatusCode)
                {
                    return resp.Content.ReadFromJsonAsync<UsuarioEnt>().Result;
                }

                return null;
            }
        }

        public int Registrarse(UsuarioEnt entidad)
        {
            using (var client = new HttpClient())
            {
                string url = "https://localhost:44351/api/Registrarse";
                JsonContent body = JsonContent.Create(entidad); //serializar
                HttpResponseMessage resp = client.PostAsync(url, body).Result;

                if (resp.IsSuccessStatusCode)
                {
                    return resp.Content.ReadFromJsonAsync<int>().Result;
                }

                return 0;
            }
        }

    }
}