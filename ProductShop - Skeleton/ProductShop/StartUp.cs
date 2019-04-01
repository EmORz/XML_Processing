using AutoMapper;
using ProductShop.Data;
using ProductShop.Dtos.Export;
using ProductShop.Dtos.Import;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<ProductShopProfile>();
            });
            //
            //var usersXml =
            //    File.ReadAllText("E:\\XML_Processing\\ProductShop - Skeleton\\ProductShop\\Datasets\\categories-products.xml");
            using (ProductShopContext context = new ProductShopContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                var data = GetCategoriesByProductsCount(context);
                Console.WriteLine(data);
            }
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {

            /*Select all users who have at least 1 sold product. Order them by the number of sold products (from highest to lowest).
             Select only their first and last name, age, count of sold products and for each product - name and price sorted by price (descending).*/
            var users = context
                .Users
                .Where(x => x.ProductsSold.Any())
                .Select(x => new ExportUserAndProductDto
                {
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Age = x.Age,
                    SoldProductDto = new SoldProductDto
                    {
                        Count = x.ProductsSold.Count,
                        ProductDtos = x.ProductsSold.Select(p => new ProductDto()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                            .OrderByDescending(p => p.Price)
                            .ToArray()
                    }
                    
                })
                .OrderByDescending(x => x.SoldProductDto.Count)
                .Take(10)
                .ToArray();

            var customExport = new ExportCustomUserProductDto
            {
                Count = context.Users.Count(x => x.ProductsSold.Any()),
                ExportUserAndProductDto = users
            };

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCustomUserProductDto), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });

            xmlSerializer.Serialize(new StringWriter(sb), customExport, namespaces);


            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context
                .Categories
                .Select(x => new ExportCategoriesByProductsCountDto
                {
                    Name = x.Name,
                    ProductCount = x.CategoryProducts.Count,
                    AveragePrice = x.CategoryProducts.Select(a => a.Product.Price).Average(),
                    TotalRevenue = x.CategoryProducts.Select(а => а.Product.Price).Sum()
                })
                .OrderByDescending(x => x.ProductCount)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();



            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCategoriesByProductsCountDto[]), new XmlRootAttribute("Categories"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });

            xmlSerializer.Serialize(new StringWriter(sb), categories, namespaces);


            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
           // Get all users who have at least 1 sold item. Order them by last name, then by first name.
           // Select the person's first and last name. For each of the sold products, select the product's name and price. Take top 5 records.
            var soldProducts = context
                .Users
                .Where(x => x.ProductsSold.Count>0)
                .OrderBy(l => l.LastName)
                .ThenBy(f => f.FirstName)
                .Take(5)
                .Select(x => new ExportSoldProductsDto
                {
                    FirstName = x.FirstName,
                    LastName =  x.LastName,
                    ProductDto = x.ProductsSold.Select( ps => new  ProductDto
                    {
                         Name = ps.Name,
                        Price = ps.Price
                    }).ToArray()
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportSoldProductsDto[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });
            xmlSerializer.Serialize(new StringWriter(sb), soldProducts, namespaces);

            return sb.ToString().Trim();
            
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            //Get all products in a specified price range between 500 and 1000(inclusive).
            //Order them by price(from lowest to highest).Select only the product name, price and the full name of the buyer.Take top 10 records.

            var products = context
                .Products
                .Where(x => x.Price >= 500 && x.Price <= 1000)
                .OrderBy(x => x.Price)
                .Select(x => new ExportProductInRangeDto
                {
                    Name = x.Name,
                    Price = x.Price,
                    FullName = x.Buyer.FirstName + " "+x.Buyer.LastName

                })
                .Take(10)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportProductInRangeDto[]), new XmlRootAttribute("Products"));

            var sb = new StringBuilder();
            
            var namespaces = new XmlSerializerNamespaces(new []
            {
                new XmlQualifiedName("", ""), 
            });

            xmlSerializer.Serialize(new StringWriter(sb), products, namespaces );


            return sb.ToString().TrimEnd();
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var validCategoriesId = context.Categories.Select(x => x.Id).ToHashSet();
            var validProductsId = context.Products.Select(x => x.Id).ToHashSet();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductsDto[]), new XmlRootAttribute("CategoryProducts"));

            var categoryProductsDtos = (ImportCategoryProductsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categoryProducts = new List<CategoryProduct>();

            foreach (var categoryProductDto in categoryProductsDtos)
            {

                var isValid = validCategoriesId.Contains(categoryProductDto.CategoryId)
                              && validProductsId.Contains(categoryProductDto.ProductId);
                if (isValid)
                {
                    var temp = Mapper.Map<CategoryProduct>(categoryProductDto);
                    categoryProducts.Add(temp);
                }
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";



        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoriesDto[]), new XmlRootAttribute("Categories"));

            var categoriesDtos = (ImportCategoriesDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var categories = new List<Category>();

            foreach (var categoryDto in categoriesDtos)
            {
                if (categoryDto.Name==null)
                {
                    continue;
                }
                var temp = Mapper.Map<Category>(categoryDto);
                categories.Add(temp);
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }


        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));

            var productsDto = (ImportProductDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var products = new List<Product>();

            foreach (var productDto in productsDto)
            {
                var product = Mapper.Map<Product>(productDto);
                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";


        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users") );

            var usersDto = (ImportUserDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var users = new List<User>();

            foreach (var userDto in usersDto)
            {
                var user = Mapper.Map<User>(userDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
    }
}