using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Guestbooks
    {
        //編號
        [DisplayName("編號:")]
        public int Id { get; set; }

        //名字
        [DisplayName("名字:")]
        [Required(ErrorMessage = "要輸入啦")]
        [StringLength(20, ErrorMessage = "名字不可超過20個字元")]
        public string Name { get; set; }

        //留言內容
        [DisplayName("留言內容:")]
        [Required(ErrorMessage = "要輸入啦")]
        [StringLength(100, ErrorMessage = "打太多字了,只能100個內")]
        public string Content { get; set; }

        //新增時間
        [DisplayName("新增時間:")]
        public DateTime CreateTime { get; set; }

        //回覆內容
        [DisplayName("回覆內容:")]
        [StringLength(100, ErrorMessage = "打太多字了,只能100個內")]
        public string Reply { get; set; }

        //DateTime?資料型態,允許DateTime有NULL產生
        [DisplayName("回覆時間:")]
        public DateTime? ReplyTime { get; set; }

    }
}
