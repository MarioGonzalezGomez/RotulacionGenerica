using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Generico_Front.Models;

namespace Generico_Front.Controllers.Data;
public class CrawlController : IBaseController<Crawl>
{
    private static CrawlController instance;
    private Cliente c;

    private CrawlController()
    {
        c = Cliente.GetInstance();
    }

    public static CrawlController GetInstance()
    {
        if (instance == null)
        {
            instance = new CrawlController();
        }
        return instance;
    }


    public List<Crawl> GetAllAsync()
    {
        string result = c.GetAsync("api/Crawls").Result;
        List<Crawl> crawl;
        if (!string.IsNullOrEmpty(result))
        {
            crawl = JsonSerializer.Deserialize<List<Crawl>>(result);
        }
        else
        {
            crawl = new List<Crawl>();
        }
        return crawl;
    }
    public Crawl GetById(int id)
    {
        string result = c.GetAsync($"api/Crawls/{id}").Result;
        Crawl crawl = JsonSerializer.Deserialize<Crawl>(result);
        return crawl;
    }
    public Crawl Post(Crawl Crawl)
    {
        string json = JsonSerializer.Serialize(Crawl);
        string result = c.PostAsync($"api/Crawls", json).Result;
        Crawl crawlResult = JsonSerializer.Deserialize<Crawl>(result);
        return crawlResult;
    }
    public Crawl Put(Crawl crawl)
    {
        string json = JsonSerializer.Serialize(crawl);
        string result = c.PutAsync($"api/Crawls/{crawl.id}", json).Result;
        return crawl;
    }
    public Crawl Delete(Crawl Crawl)
    {
        string result = c.DeleteAsync($"api/Crawls/{Crawl.id}").Result;
        return Crawl;
    }
}
