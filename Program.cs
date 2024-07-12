using System.Net;
using System.Security.AccessControl;
using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.Exceptions;

namespace MinioTest
{
    internal class Program
    {
        //upload file , lấy thông tin file
        static async Task Main(string[] args)
        {
            // Cấu hình bảo mật TLS
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Tls12
                | SecurityProtocolType.Tls11;

            var endPoint = "localhost:9000";
            var accessKey = "BIjpvvvjYdNvRoEURXWF";
            var secretKey = "Xr0yUqKN4WPrQ6NMTGYhF8sueymSzvbuad9VHIB0";

            var bucketName = "pdf-bucket";
            var filePath = @"C:\Users\phamv\Downloads\[O`Reilly. Head First] - Head First Design Patterns - [Freeman].pdf";
            var objectName = "DesignPattern.pdf";

            var minioHelper = new MinioHepler(endPoint, accessKey, secretKey);

            try
            {
                //nếu ko dùng await chương trình sẽ ko đợi DisplayMenuAsync thực hiện mà kết thức luôn
                await DisplayMenuAsync(minioHelper);
                Console.WriteLine("OK");
            }
            catch (MinioException minioEx)
            {
                Console.WriteLine($"MinIO Error: {minioEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }











        }

        static async Task DisplayMenuAsync(MinioHepler minioHelper)
        {
            while (true)
            {
                Console.WriteLine("=== MinIO Operations Menu ===");
                Console.WriteLine("1. List Buckets");
                Console.WriteLine("2. Create Bucket");
                Console.WriteLine("3. Upload File");
                Console.WriteLine("4. Delete File");
                Console.WriteLine("5. Show info file");
                Console.Write("Choose an option (1-5): ");
                var choice = Console.ReadLine();
                string bucketName, objectName;

                switch (choice)
                {
                    case "1":
                        await minioHelper.ShowListBucket();
                        break;
                    case "2":
                        await minioHelper.CreateBucketAsync();
                        break;
                    case "3":
                        await minioHelper.UploadFileAsync();
                        break;
                    case "4":
                        await minioHelper.DeleteFileAsync();
                        break;
                    case "5":
                        await minioHelper.GetFileInfoAsync();
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please choose again.");
                        break;
                }
            }
        }
    }
}
