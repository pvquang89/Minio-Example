using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Minio;
using Minio.ApiEndpoints;
using Minio.DataModel.Args;


namespace MinioTest
{
    public class MinioHepler
    {
        private readonly MinioClient _minioClient;

        public MinioHepler(string endPoint, string accessKey, string secretKey)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                                | SecurityProtocolType.Tls11;
            _minioClient = (MinioClient?)new MinioClient()
                .WithEndpoint(endPoint)
                .WithCredentials(accessKey, secretKey)
                .Build();
        }

        //kiểm tra bucket với tên tạo đã tồn tại hay chưa
        public async Task<bool> IsBucketExist(string bucketName)
        {
            var bucketExist = new BucketExistsArgs().WithBucket(bucketName);
            return await _minioClient.BucketExistsAsync(bucketExist);
        }

        //Tạo bucket
        public async Task CreateBucketAsync()
        {
            Console.Write("Enter bucket name: ");
            var bucketName = Console.ReadLine().Trim();
            bool found = await IsBucketExist(bucketName);
            if (!found)
            {

                var bucket = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(bucket);
                Console.WriteLine($"Bucket '{bucketName}' created successfully.");
            }
            else
            {
                Console.WriteLine($"Bucket '{bucketName}' is exist.");
            }
        }

        public async Task ShowListBucket()
        {
            var bucketsResult = await _minioClient.ListBucketsAsync();
            if (bucketsResult != null)
            {

                for (int i = 0; i < bucketsResult.Buckets.Count; i++)
                {
                    Console.WriteLine($"Bucket {i + 1}: {bucketsResult.Buckets[i].Name}");
                }
                //foreach (var b in bucketsResult.Buckets)
                //{
                //    Console.WriteLine($"Bucket name : {b.Name}");
                //}
            }
        }

        //upload file
        public async Task UploadFileAsync()
        {
            await ShowListBucket();
            Console.Write("Enter the number of the bucket you want to use : ");
            int i = int.Parse(Console.ReadLine());
            var bucketsResult = await _minioClient.ListBucketsAsync();
            var bucketName = bucketsResult.Buckets[i - 1].Name;
            Console.Write("Bucket name: " + bucketName);
            Console.Write("\nEnter local file path: ");
            var filePath = Console.ReadLine().Trim();
            var filePathFinal = filePath.Substring(1, filePath.Length - 2);
            Console.Write("Enter content type (e.g., application/pdf): ");
            var contentType = Console.ReadLine().Trim();
            var objectName = System.IO.Path.GetFileName(filePathFinal); // Sử dụng tên file từ đường dẫn file
            var obj = new PutObjectArgs()
                .WithBucket(bucketName)         //tên bucket
                .WithObject(objectName)         //tên file 
                .WithFileName(filePathFinal)         //đường dẫn tới file
                .WithContentType(contentType);  //loại file
            //upload file lên minio
            await _minioClient.PutObjectAsync(obj);
            Console.WriteLine($"File '{objectName}' uploaded successfully to bucket '{bucketName}'.");
        }

        //xoá file trong bucket
        public async Task DeleteFileAsync()
        {
            await ShowListBucket();
            Console.Write("Enter the number of the bucket you want to use : ");
            int i = int.Parse(Console.ReadLine());
            var bucketsResult = await _minioClient.ListBucketsAsync();
            var bucketName = bucketsResult.Buckets[i - 1].Name;
            Console.Write("Bucket name: " + bucketName);
            Console.Write("Enter object name to delete: ");
            var objectName = Console.ReadLine().Trim();
            var obj = new RemoveObjectArgs().WithBucket(bucketName).WithObject(objectName);
            await _minioClient.RemoveObjectAsync(obj);
            Console.WriteLine($"File '{objectName}' deleted successfully from bucket '{bucketName}'.");
        }
    
        //lấy thông tin file
        public async Task GetFileInfoAsync()
        {
            Console.Write("Enter bucket name: ");
            var bucketName = Console.ReadLine().Trim();
            Console.Write("Enter object name : ");
            var objectName = Console.ReadLine().Trim();
            var statObjectArgs = new StatObjectArgs().WithBucket(bucketName).WithObject(objectName);
            var statObjectResponse = await _minioClient.StatObjectAsync(statObjectArgs);
            Console.WriteLine($"File '{objectName}' info: {statObjectResponse}");
        }

    }
}
