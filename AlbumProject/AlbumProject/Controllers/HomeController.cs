using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using Aliyun.OSS.Util;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using System.IO;
using System.Threading;
using System.Security.Cryptography;
using System.Globalization;

namespace AlbumProject.Controllers
{
    
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            /// <summary>
            /// 在OSS中创建一个新的存储空间。
            /// </summary>
            /// <param name="bucketName">要创建的存储空间的名称</param>
            PutObjectSample1 creatbu = new PutObjectSample1();
            //creatbu.CreatBucket("igetsss");
            return View();
        }
        public void CheckAll()
        {
            PutObjectSample1 creatbu = new PutObjectSample1();
            //creatbu.GetObject1();
            //var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
            PutObjectSample1.ImageProcess("flowera");
        }

    }
    public class PutObjectSample1
    {
        //创建个人的AccessKeyId
        string accessKeyId = "LTAId7dsrQHujhU5";
        string accessKeySecret = "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p";
        string endpoint = "oss-cn-shenzhen.aliyuncs.com";


        //初始化OssClient并创造存储空间
        public void CreatBucket(string bucketName)
        {
            ////其他参数
            //ClientConfiguration conf = new ClientConfiguration();
            //conf.MaxErrorRetry = 3;
            //conf.ConnectionTimeout = 300;
            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var bucket = client.CreateBucket("igetsss");
                Console.Write("成功创建");
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
        }

        //上传本地文件路径（文件名最好加后缀.jpg）
        public string fileUp()
        {
            OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var result = client.PutObject("igets", "pics.jpg", "D:/Users/pc/Desktop/2.jpg");
                return "上传成功！";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //查看相册中有哪些相片(名称、时间、以及存储空间)，可用于用户来查看相片数和相片名称
        public void ListObjects()
        {
            int count = 0;
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var listObjectsRequest = new ListObjectsRequest("igets");
                var result = client.ListObjects(listObjectsRequest);
                Console.WriteLine("成功检索出这些文件：");
                foreach (var summary in result.ObjectSummaries)
                {
                    count++;
                    Console.WriteLine(summary.Key);
                    //summary.Key是文件名
                    //summary.LastModified是文件上传时间
                    //summary.BucketName是存储空间名字（相当于相册名）
                }
                Console.WriteLine(count + "张相片");//显示此相册中相片数目有多少
            }
            catch (Exception ex)
            {
                Console.WriteLine("List object failed, {0}", ex.Message);
            }
        }

        //下载文件（相当于客户下载相片，可多图下载）
        public void GetObject1()
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                string[] names = new string[] { "banner.jpg", "pics.jpg" };//根据用户选择图片的多少来自定义

                //将从OSS读取到的文件写到本地并循环文件集
                for (int i = 0; i < names.Length; i++)
                {
                    var obj = client.GetObject("igets", names[i]);
                    using (var requestStream = obj.Content)
                    {
                        byte[] buf = new byte[1024];
                        using (var fs = File.Open("E:/Content/" + names[i] + "", FileMode.OpenOrCreate))//此处，必须要加name（作为下载文件的名字）,否则会显示“对路径的存储错误”
                        {
                            var len = 0;
                            while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                            {
                                fs.Write(buf, 0, len);
                            }
                        }
                    }
                }
                
                Console.WriteLine("成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //删除文件（相当于客户删除相片，可删除多张图片）
        public void DeleteObject()
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                string[] names = new string[] { "pic.jpg", "2.jpg" };
                for (int i = 0; i < names.Length; i++)
                {
                    client.DeleteObject("flowera", names[i]);
                }
                Console.WriteLine("删除成功");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete object failed, {0}", ex.Message);
            }
        }

        //列举出所有的存储空间
        public void GetAlbums()
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var buckets = client.ListBuckets();
                Console.WriteLine("所有相册：");
                foreach (var bucket in buckets)
                {
                    Console.WriteLine("相册名称：{0}，地址：{1}，物主：{2}", bucket.Name, bucket.Location, bucket.Owner);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("List bucket failed. {0}", ex.Message);
            }
        }

        //判断存储空间是否存在（相当于判断相片是否存在，可用于客户查询）
        public void IfAlbumExit()
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var exist = client.DoesBucketExist("igets");
                Console.WriteLine("查询是否存在");
                Console.WriteLine("存在 ? {0}", exist);
            }
            catch (Exception ex)
            {
                Console.WriteLine( ex.Message);
            }
        }

        //获取存储空间的访问权限
        public void GetBucketAcl()
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                var acl = client.GetBucketAcl("igets");
                Console.WriteLine("成功获取权限");
                foreach (var grant in acl.Grants)
                {
                    Console.WriteLine("获取存储空间权限成功，当前权限:{0}", grant.Permission.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get bucket ACL failed. {0}", ex.Message);
            }
        }

        //删除存储空间
        public void DeleteBucket(string bucketName)
        {
            var client = new OssClient(endpoint, accessKeyId, accessKeySecret);
            try
            {
                client.DeleteBucket(bucketName);
                Console.WriteLine("删除成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        //异步上传文件
        #region
        static AutoResetEvent _event = new AutoResetEvent(false);
        public static void AsyncPutObject()
        {
            var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
            try
            {
                using (var fs = File.Open("D:/Users/pc/Desktop/个人文件/AlbumProject/AlbumProject/Content/2.jpg", FileMode.Open))
                {
                    var metadata = new ObjectMetadata();
                    metadata.CacheControl = "No-Cache";
                    metadata.ContentType = "text/html";
                    client.BeginPutObject("flowera", "pic.jpg", fs, metadata, PutObjectCallback, new string('a', 8));
                    _event.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);
            }
        }
        private static void PutObjectCallback(IAsyncResult ar)
        {
            var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
            try
            {
                var result = client.EndPutObject(ar);
                Console.WriteLine("ETag:{0}", result.ETag);
                Console.WriteLine("User Parameter:{0}", ar.AsyncState as string);
                Console.WriteLine("Put object succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);
            }
            finally
            {
                _event.Set();
            }
        }
        #endregion

        //追加文件（防止添加重复相片）
        public static void AppendObject()
        {
            //第一次追加文件的时候，文件可能已经存在，先获取文件的当前长度，如果不存在，position为0，如果存在则不会添加进去
            long position = 0;
            try
            {
                var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
                var metadata = client.GetObjectMetadata("flowera", "2.jpg");
                position = metadata.ContentLength;
            }
            catch (Exception) { }
            try
            {
                var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
                using (var fs = File.Open("D:/Users/pc/Desktop/个人文件/AlbumProject/AlbumProject/Content/2.jpg", FileMode.Open))
                {
                    var request = new AppendObjectRequest("flowera", "2.jpg")
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };
                    var result = client.AppendObject(request);
                    // 设置下次追加文件时的position位置
                    position = result.NextAppendPosition;
                    Console.WriteLine("Append object succeeded, next append position:{0}", position);
                }
                // 再次追加文件，这时候的position值可以从上次的结果中获取到
                using (var fs = File.Open("D:/Users/pc/Desktop/个人文件/AlbumProject/AlbumProject/Content/2.jpg", FileMode.Open))
                {
                    var request = new AppendObjectRequest("flowera", "2.jpg")
                    {
                        ObjectMetadata = new ObjectMetadata(),
                        Content = fs,
                        Position = position
                    };
                    var result = client.AppendObject(request);
                    position = result.NextAppendPosition;
                    Console.WriteLine("Append object succeeded, next append position:{0}", position);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Append object failed, {0}", ex.Message);
            }
        }

        //搜寻照片
        public void SearchPhoto()
        {
            try
            {
                var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
                var listObjectsRequest = new ListObjectsRequest("igets")
                {
                    Prefix = "banner"//想搜索的照片名称
                };
                var result = client.ListObjects(listObjectsRequest);
                Console.WriteLine("List objects succeeded");
                foreach (var summary in result.ObjectSummaries)
                {
                    Console.WriteLine("File Name:{0}", summary.Key);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("List objects failed. {0}", ex.Message);
            }
        }

        //创建相册
        public void CreateFile()
        {
            try
            {
                var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
                // 重要: 这时候作为目录的key必须以斜线（／）结尾
                const string key = "newfloder/";
                // 此时的目录是一个内容为空的文件
                using (var stream = new MemoryStream())
                {
                    client.PutObject("igets", key, stream);
                    Console.WriteLine("生成成功", key);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Create dir failed, {0}", ex.Message);
            }
        }

        //删除相册（相册名后面加"/"）
        public void DeleteObjectss()
        {
            var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
            try
            {
                client.DeleteObject("igets", "adad/");
                Console.WriteLine("Delete object succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Delete object failed, {0}", ex.Message);
            }
        }

        //
        public static void GetBucketLifecycle()
        {
            var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
            try
            {
                var rules = client.GetBucketLifecycle("igets");

                Console.WriteLine("Get bucket:{0} Lifecycle succeeded ", "igets");

                foreach (var rule in rules)
                {
                    Console.WriteLine("ID: {0}", rule.ID);
                    Console.WriteLine("Prefix: {0}", rule.Prefix);
                    Console.WriteLine("Status: {0}", rule.Status);
                    if (rule.ExpriationDays.HasValue)
                        Console.WriteLine("ExpirationDays: {0}", rule.ExpriationDays);
                    if (rule.ExpirationTime.HasValue)
                        Console.WriteLine("ExpirationTime: {0}", FormatIso8601Date(rule.ExpirationTime.Value));
                }
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }

        private static string FormatIso8601Date(DateTime dateTime)
        {
            return dateTime.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'",
                               CultureInfo.CreateSpecificCulture("en-US"));
        }

        //改变存储空间的访问权限
        public void SetBucketAcl(string buckteName)
        {
            var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
            try
            {
                // 指定Bucket ACL为公共读
                client.SetBucketAcl(buckteName, CannedAccessControlList.PublicRead);//修改为公共读
                //client.SetBucketAcl(buckteName, CannedAccessControlList.Private);//修改为私有读
                Console.WriteLine("修改成功！");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Set bucket ACL failed. {0}", ex.Message);
            }
        }

        //生成暂时带签名的URL
        public static void GenerateIamgeUri()
        {
            var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
            try
            {
                var req = new GeneratePresignedUriRequest("igets", "igetsLogo.jpg", SignHttpMethod.Get)
                {
                    Expiration = DateTime.Now.AddSeconds(30 * 60),
                };
                // 产生带有签名的URI
                var uri = client.GeneratePresignedUri(req);
                Console.WriteLine("Uri:{0}", uri);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }
        }

        //图片处理(文字水印)
        public static void WatermarkImage(string bucketName)
        {
            var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
                client.PutObject("flowera", "newimg.jpg", "D:/Users/pc/Desktop/ssss.jpg");//将本地地址提交到存储空间

                // 图片加文字水印
                var process = "image/watermark,text_SGVsbG8g5Zu-54mH5pyN5YqhIQ";//自定义水印
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, "newimg.jpg", process));//然后对图片添加水印

                WriteToFile("E:/Content/xixi.jpg", ossObject.Content);//将添加完水印的图片保存下来

                Console.WriteLine("Get Object:{0} with process:{1} succeeded ", "2.jpg", process);
            
        }
        private static void WriteToFile(string filePath, Stream stream)
        {
            using (var requestStream = stream)
            {
                using (var fs = File.Open(filePath, FileMode.OpenOrCreate))
                {
                    IoUtils.WriteTo(stream, fs);
                }
            }
        }

        public static void ImageProcess(string bucketName)
        {
            var client = new OssClient("oss-cn-shenzhen.aliyuncs.com", "LTAId7dsrQHujhU5", "O3nQOqai4yXrvGCKNbvgrKuU8f7U7p");
                //client.PutObject("flowera", "newimg.jpg", "D:/Users/pc/Desktop/ssss.jpg");//直接下载库中的也可以下载
                // 自定义样式
                var process = "style/forme";//在OSS中添加样式即可使用
                var ossObject = client.GetObject(new GetObjectRequest(bucketName, "2.jpg", process));
                WriteToFile("E:/Content/xixi.jpg", ossObject.Content);
            
        }

    }

}