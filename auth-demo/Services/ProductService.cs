using auth_demo.Models;
using System.Text.Json;

namespace auth_demo.Services
{
    public class ProductService
    {
        private readonly string _filePath = "products.json";

        public ProductService()
        {
            if (!File.Exists(_filePath))
            {
                File.WriteAllText(_filePath, "[]");
            }
        }

        public List<Products> GetAll()
        {
            var jsonData = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<Products>>(jsonData) ?? new List<Products>();
        }

        public Products? GetById(int id)
        {
            var products = GetAll();
            return products.FirstOrDefault(p => p.Id == id);
        }

        public void Add(Products product)
        {
            var products = GetAll();
            product.Id = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
            products.Add(product);
            SaveData(products);
        }

        public void Update(Products product)
        {
            var products = GetAll();
            var existingProduct = products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                SaveData(products);
            }
        }

        public void Delete(int id)
        {
            var products = GetAll();
            var product = products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                SaveData(products);
            }
        }

        private void SaveData(List<Products> products)
        {
            var jsonData = JsonSerializer.Serialize(products);
            File.WriteAllText(_filePath, jsonData);
        }
    }
}


