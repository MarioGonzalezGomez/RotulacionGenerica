using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Generico_Front.Graphics.Conexion;
public class PrimeConexion
{
    private static PrimeConexion? instance;
    public bool activo = false;
    private Socket client;
    private string _ip;
    private int _port;
    private Config.Config config;

    private PrimeConexion()
    {
        config = Config.Config.GetInstance();
        _ip = config.BrainStormOptions.Ip;
        _port = int.Parse(config.BrainStormOptions.Port);
    }

    public static PrimeConexion GetInstance()
    {
        if (instance == null)
        {
            instance = new PrimeConexion();
        }
        return instance;
    }

    public void AbrirConexion()
    {
        Console.WriteLine("Iniciando conexión...");
        try
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.Connect(_ip, _port);
            activo = true;
            Console.WriteLine($"Conectado con éxito a Prime en ip {_ip}, puerto {_port}");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            activo = false;
        }
    }

    public void EnviarMensaje(string mensaje)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(mensaje);
        client.Send(bytes);
        Console.WriteLine($"{mensaje}");
    }

    public void CerrarConexion()
    {
        client.Close();
        activo = false;
    }
}
