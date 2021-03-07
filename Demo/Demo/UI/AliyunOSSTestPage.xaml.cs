using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Aliyun.OSS;

using HB.FullStack.XamarinForms;
using HB.FullStack.XamarinForms.Files;
using HB.FullStack.XamarinForms.Platforms;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AliyunOSSTestPage : ContentPage
    {
        private OssClient _ossClient;

        public AliyunOSSTestPage()
        {
            InitializeComponent();

            _ossClient = new OssClient("oss-cn-hangzhou.aliyuncs.com", "xxx", "yy");

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                //下载
                //IEnumerable<Bucket> buckets = _ossClient.ListBuckets();
            }
            catch (HttpRequestException ex)
            {
                //403权限不足
            }

            using OssObject obj = _ossClient.GetObject(bucketName: "dev-ahabit", key: "Test.zip");

            string? fullPath = await LocalFileServiceHelper.SaveFileAsync(obj.Content, Path.Combine(FileSystem.AppDataDirectory, "Test"), obj.Key).ConfigureAwait(false);

        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            //上传
            using Stream stream = await FileSystem.OpenAppPackageFileAsync("Test.zip").ConfigureAwait(false);

            string? fullPath = await LocalFileServiceHelper.SaveFileAsync(stream, Path.Combine(FileSystem.AppDataDirectory, "Test"), "Test.zip").ConfigureAwait(false);


            using PutObjectResult result = _ossClient.PutObject(bucketName: "dev-ahabit", key: "Test.zip", fileToUpload: fullPath);

            //处理result


        }


    }
}