using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Aliyun.OSS;

using HB.FullStack.XamarinForms.Platforms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Demo.UI
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AliyunOSSTestPage : ContentPage
    {
        private OssClient _ossClient;
        private IFileHelper _fileHelper;

        public AliyunOSSTestPage()
        {
            InitializeComponent();

//            AccessKey ID
//LTAI4G2LFJ79qZUCSSGDMALM
//AccessKey Secret
//H8BjcZWXiaBYC8jAbIwok4dJQm4wAy

            _ossClient = new OssClient("oss-cn-hangzhou.aliyuncs.com", "LTAI4G2LFJ79qZUCSSGDMALM", "H8BjcZWXiaBYC8jAbIwok4dJQm4wAy");

            _fileHelper = DependencyService.Resolve<IFileHelper>();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                //下载
                //IEnumerable<Bucket> buckets = _ossClient.ListBuckets();
            }
            catch(HttpRequestException ex)
            {
                //403权限不足
            }

            using OssObject obj = _ossClient.GetObject(bucketName: "dev-ahabit", key: "Test.zip");

            string fullPath = await _fileHelper.SaveFileAsync(obj.Content, obj.Key, UserFileType.Cache).ConfigureAwait(false);

        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            //上传
            using Stream stream = _fileHelper.GetAssetStream("Test.zip");

            string fullPath = await _fileHelper.SaveFileAsync(stream, "Test.zip", UserFileType.Cache).ConfigureAwait(false);


            using PutObjectResult result = _ossClient.PutObject(bucketName: "dev-ahabit", key: "Test.zip", fileToUpload: fullPath);

            //处理result


        }
    }
}