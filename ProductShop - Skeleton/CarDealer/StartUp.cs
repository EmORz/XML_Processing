using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper;
using CarDealer.Data;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using Castle.Core.Internal;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(x =>
            {
                x.AddProfile<CarDealerProfile>();
            });
            //
            var usersXml =
                File.ReadAllText("E:\\XML_Processing\\XML_Processing\\ProductShop - Skeleton\\CarDealer\\Datasets\\cars.xml");
            using (CarDealerContext context = new CarDealerContext())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                var data = GetLocalSuppliers(context);
                Console.WriteLine(data);
            }

        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            //Get all suppliers that do not import parts from abroad. Get their id, name and the number of parts they can offer to supply. 

            var localSuppliers = context
                .Suppliers
                .Where(x => x.IsImporter == false)
                .Select(x => new ExportLocalSuppliersDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Parts_count = x.Parts.Count
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportLocalSuppliersDto[]), new XmlRootAttribute("suppliers"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });

            xmlSerializer.Serialize(new StringWriter(sb), localSuppliers, namespaces);


            return sb.ToString().TrimEnd();
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            //Get all cars from make BMW and order them by model alphabetically and by travelled distance descending.
            var bmw = context
                .Cars
                .Where(m => m.Make == "BMW")
                .Select(x => new ExportCarsModelBMW
                {
                    Id = x.Id,
                    Model = x.Model,
                    Travelled_distanc = x.TravelledDistance
                })
                .OrderBy(z => z.Model)
                .ThenByDescending(td => td.Travelled_distanc)
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarsModelBMW[]), new XmlRootAttribute("Cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });

            xmlSerializer.Serialize(new StringWriter(sb), bmw, namespaces);


            return sb.ToString().TrimEnd();

        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            //Get all cars with distance more than 2,000,000.
            //Order them by make, then by model alphabetically.
            //Take top 10 records.

            var carsWithDistance = context
                .Cars
                .Where(x => x.TravelledDistance > 2000000)
                .OrderBy(x => x.Model)
                .Select(x => new ExportCarsWithDistance
                {
                    Make = x.Make,
                    Model = x.Model,
                    Travelled = x.TravelledDistance
                })
                .Take(10)
                .ToArray();


            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportCarsWithDistance[]), new XmlRootAttribute("Cars"));

            var sb = new StringBuilder();

            var namespaces = new XmlSerializerNamespaces(new[]
            {
                new XmlQualifiedName("", ""),
            });

            xmlSerializer.Serialize(new StringWriter(sb), carsWithDistance, namespaces);


            return sb.ToString().TrimEnd();
        }



        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            //l. If car doesn’t exists, skip whole entity.
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSalesDto[]), new XmlRootAttribute("Sales"));

            var salesDto = (ImportSalesDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var sales = new List<Sale>();

            foreach (var saleDto in salesDto)
            {
                //l. If car doesn’t exists, skip whole entity.
                var carExist = context.Cars.Find(saleDto.CarId);
           
                var user = Mapper.Map<Sale>(saleDto);
                sales.Add(user);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";

        }


        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomersDto[]), new XmlRootAttribute("Customers"));

            var customersDtos = (ImportCustomersDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var customers = new List<Customer>();

            foreach (var customerDto in customersDtos)
            {
                var temp = Mapper.Map<Customer>(customerDto);
                customers.Add(temp);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";

        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            //Select unique car part ids. If the part id doesn’t exists, skip the part record.

            //todo dont work!!!
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarsDto[]), new XmlRootAttribute("Cars"));

            var carsDtos = (ImportCarsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var cars = new List<Car>();

            foreach (var carDto in carsDtos)
            {
                
                var temp = Mapper.Map<Car>(carDto);
                cars.Add(temp);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }


        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            //todo 50/100
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartsDto[]), new XmlRootAttribute("Parts"));

            var partsDtos = (ImportPartsDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var parts = new List<Part>();

            foreach (var partDto in partsDtos)
            {
                // If the supplierId doesn’t exists, skip the record.
                if (partDto.SupplierId==null)
                {
                    continue;
                }
                var temp = Mapper.Map<Part>(partDto);
                parts.Add(temp);
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";

        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSuppliersDto[]), new XmlRootAttribute("Suppliers"));

            var suppliersDto = (ImportSuppliersDto[])xmlSerializer.Deserialize(new StringReader(inputXml));

            var suppliers = new List<Supplier>();

            foreach (var supplierDto in suppliersDto)
            {
                var user = Mapper.Map<Supplier>(supplierDto);
                suppliers.Add(user);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }
    }
}