using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VietChamLearn_BuiVanChat_18607075.Models;
using Newtonsoft.Json;
using PagedList;


using Tesseract;

namespace VietChamLearn_BuiVanChat_18607075.Controllers
{
    public class DefaultController : Controller
    {
        private VietChamLearn_BuiVanChat_18607075Entities db = new VietChamLearn_BuiVanChat_18607075Entities();
        // GET: Default
        public ActionResult Index(string HidenDichCau)
        {

            ViewBag.Title = "Bùi Văn Chắt ocr";
            var listwords = new List<string>();
            if (!string.IsNullOrEmpty(HidenDichCau))
            {
                var list = HidenDichCau.Split(',').ToList();
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        //var i = new string();
                        //i = string(item);
                        //listwords.Add(i);
                    }


                }
            }


            return View();
        }

        // public JsonResult Translate(string HidenDichCau )
        //{
        //    //wordvietTrans()
        //    if (!string.IsNullOrEmpty(HidenDichCau))
        //    {
        //        var listwords = new List<int>();
        //        //Li i = new List<string>();
        //        var listt = HidenDichCau.Split(',').ToList();
        //        if (listt != null && listt.Count > 0)
        //        {

        //            //var TransDone
        //            foreach (var item in listt)
        //            {
        //                //    //var i = ;
        //                //    //  i = wordvietTrans(item);
        //                //    //var i = new string();

        //                        var i = new int();
        //                        var w = wordvietTrans(item);
        //                        i = Int32.Parse(w.ToString());
        //                //    //wordvietTrans(item);
        //                //i = Int32.Parse( w);
        //                //var c = wordvietTrans(i);
        //                         listwords.Add(i);
        //                //listwords.Add(db.Words.Where(x => x.Viet.Contains(item)).Select(x => x.Ede).ToList();
        //                   // var convert = JsonConvert.DeserializeObject(listwords).ToString();
        //            //    //var listwords = new List<string>(wordvietTrans(item)).ToList();


        //            }
        //            //var convert = JsonConvert.SerializeObject(listwords).ToString();
        //            //var result = JsonConvert.DeserializeObject<List<Word>>(convert);
        //            //listwords.AddRange(Enumerable.Range(0, 5).Select(i => i.ToString().ToArray());
        //            //index.Add(linesOfContent);
        //            //ViewBag.listwordss = listwords;
        //            return Json(listwords, JsonRequestBehavior.AllowGet);
        //        }
        //    }
        //    //var data = wordvietTrans(HidenDichCau);
        //    return Json(0);
        //    //{
        //    //    data = data,
        //    //    status = true
        //    //}, JsonRequestBehavior.AllowGet

        //}
        public JsonResult TranslateID(string HidenDichCau)
        {
            var listId = new List<string>();
            if (!string.IsNullOrEmpty(HidenDichCau))
            {
                var list = HidenDichCau.Split(',').ToList();
                if (list != null && list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        //int w = wordvietTransID(item);
                        //int i = new int();
                        //i = Int32.Parse(item);
                        listId.Add(item);

                    }
                }
            }
            var obj = db.Words.Where(x => listId.Contains(x.Viet_Word)).Select(x => x.Cham_Word).ToList().Split(' ');
            ViewBag.Text = obj;
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public List<string> wordvietTransID(string list)
        {
            return db.Words.Where(x => x.Viet_Word.Contains(list)).Select(x => x.Cham_Word).ToList();
        }
        //[AllowAnonymous]
        public ActionResult submitOcr(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                ViewBag.Result = true;
                ViewBag.res = "Không có tệp nào được tải lên!! vui lòng kiểm tra lại!!! Thanks";
                return View();
            }
            using (var engine = new TesseractEngine(Server.MapPath(@"~/tessdata"), "vie", EngineMode.Default))
            {
                using (var image = new System.Drawing.Bitmap(file.InputStream))
                {
                    using (var pix = PixConverter.ToPix(image))
                    {
                        using (var page = engine.Process(pix))
                        {
                            ViewBag.Result = true;
                            var pageImg = page.GetText();
                            var pageTxt = pageImg.TrimEnd('\n');//RA ĐƯỢC CHỮ TỪ ẢNH
                           /* var TxtImg = pageTxt.Split(' '); */
                            var listId = new List<string>(); //KHAI BÁO MỘT CHUỖI STRING MỚI CHỨA ID CỦA CÁC CHỮ ĐÃ TÁCH RA
                            if (!string.IsNullOrEmpty(pageTxt))
                            {
                                var list = pageTxt.Split(' ').ToList(); // TÁCH CHỮ RA KHỎI CHUỖI
                                if (list != null && list.Count > 0)
                                {
                                    foreach (var item in list)
                                    {
                                        
                                        listId.Add(item); //lưu vào ListId

                                    }
                                }
                            }
                            var chuoi = new List<string>(); //khai bao 1 list string mới để chứa chuỗi string khi + vào khaonr cách
                            var text = db.Words.Where(x => listId.Contains(x.Viet_Word)).Select(x => x.Cham_Word).ToList();//truy vấn
                            if (text.Count >0 || text!= null)
                            {
                                foreach(var item in text)
                                {
                                    var chu = item + " ";
                                    chuoi.Add(chu); //add chữ vào chuỗi list string 
                                }
                                
                            }

                            var obj = string.Concat(chuoi).ToString(); // conver json to string

                            ViewBag.res = obj;
                            //var catchuoi = Convert.ToString(page.GetText().Split(new char[] { ' ', ',', '.', ':', '-' }));
                            ViewBag.mean = String.Format("{0:p}", page.GetMeanConfidence());
                            return View();

                        }
                    }
                }
            }
            //return View();
        }
        //public JsonResult submitOcr(HttpPostedFileBase file )
        //{
        //    if (file == null || file.ContentLength == 0)
        //    {
        //        ViewBag.Result = true;
        //        ViewBag.res = "Không có tệp nào được tải lên!! vui lòng kiểm tra lại!!! Thanks";
        //        return Json(JsonRequestBehavior.AllowGet);
        //    }
        //    using (var engine = new TesseractEngine(Server.MapPath(@"~/tessdata"), "vie", EngineMode.Default))
        //    {
        //        using (var image = new System.Drawing.Bitmap(file.InputStream))
        //        {
        //            using (var pix = PixConverter.ToPix(image))
        //            {
        //                using (var page = engine.Process(pix))
        //                {
        //                    ViewBag.Result = true;
        //                    var pageImg = page.GetText();
        //                    var pageTxt = pageImg.TrimEnd('\n');
        //                    var TxtImg = pageTxt.Split(' ');
        //                    var listId = new List<string>();
        //                    if (!string.IsNullOrEmpty(pageTxt))
        //                    {
        //                        var list = pageTxt.Split(',').ToList();
        //                        if (list != null && list.Count > 0)
        //                        {
        //                            foreach (var item in list)
        //                            {
        //                                //int w = wordvietTransID(item);
        //                                //int i = new int();
        //                                //i = Int32.Parse(item);
        //                                listId.Add(item);

        //                            }
        //                        }
        //                    }
        //                    var obj = db.Words.Where(x => listId.Contains(x.Viet)).Select(x => x.Ede).ToList();

        //                    //ViewBag.res = obj;
        //                    //var catchuoi = Convert.ToString(page.GetText().Split(new char[] { ' ', ',', '.', ':', '-' }));
        //                    ViewBag.mean = String.Format("{0:p}", page.GetMeanConfidence());
        //                    return Json(obj, JsonRequestBehavior.AllowGet);

        //                }
        //            }
        //        }
        //    }
        //    //return View();
        //}

        //[AllowAnonymous]
        //public JsonResult submitOcr2(HttpPostedFileBase file)
        //{
        //    if (file == null || file.ContentLength == 0)
        //    {
        //        ViewBag.Result = true;
        //        ViewBag.res = "Không có tệp nào được tải lên!! vui lòng kiểm tra lại!!! Thanks";
        //        return Json(JsonRequestBehavior.AllowGet);
        //    }
        //    using (var engine = new TesseractEngine(Server.MapPath(@"~/tessdata"), "vie", EngineMode.Default))
        //    {
        //        using (var image = new System.Drawing.Bitmap(file.InputStream))
        //        {
        //            using (var pix = PixConverter.ToPix(image))
        //            {
        //                using (var page = engine.Process(pix))
        //                {
        //                    ViewBag.Result = true;
        //                    ViewBag.res = page.GetText();
        //                    ViewBag.mean = String.Format("{0:p}", page.GetMeanConfidence());
        //                    string catchuoi = Convert.ToString(page.GetText().Split(new char[] { ' ', ',', '.', ':', '-' }));
        //                    return Json(JsonRequestBehavior.AllowGet);

        //                }
        //            }
        //        }
        //    }
        //    //return View();
        //}


        //public ActionResult Search(string search_input)
        //{
        //    var trans = db.Words.Where(x => x.Viet.Contains(search_input)).Select(x => x.Viet).ToList();
        //    return Json(trans, JsonRequestBehavior.AllowGet);
        //}
        public JsonResult GetWordsVieEde(string term)
        {

            List<string> words = db.Words.Where(s => s.Viet_Word.StartsWith(term))

                .Select(x => x.Viet_Word ).ToList();

            return Json(words, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWordsEdeVie(string term)
        {

            List<string> words = db.Words.Where(s => s.Cham_Word .StartsWith(term))

                .Select(x => x.Cham_Word ).ToList();

            return Json(words, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetWordsEdeViet(string keyword)
        {
            ViewBag.ede = keyword;
            var model = GetWordsEdeVietSearch(keyword);
            return View(model);
        }
        public List<Word> GetWordsEdeVietSearch(string keyword)
        {
            var model = (from a in db.Words
                             //join b in db.Categories
                             //on a.id_cate equals b.Id
                         where a.Cham_Word.Contains(keyword)
                         select new
                         {
                             Cham_Word = a.Cham_Word,
                             Viet_Word = a.Viet_Word,
                             Description_Word = a.Description_Word,

                         }).AsEnumerable().Select(x => new Word()
                         {
                             Cham_Word = x.Cham_Word,
                             Viet_Word = x.Viet_Word,
                             Description_Word = x.Description_Word,
                         });

            model.OrderByDescending(x => x.Description_Word);
            return model.ToList();
        }


        public List<string> wordede(string keyword)
        {
            return db.Words.Where(x => x.Cham_Word.Contains(keyword)).Select(x => x.Cham_Word).ToList();
        }

        public JsonResult ListWordEde(string q)
        {
            //List<string> words = db.Words.Where(s => s.Ede.StartsWith(q))

            //    .Select(x => x.Ede).ToList();
            var data = wordede(q);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWordsVietEde(string keyword1)
        {
            ViewBag.viet = keyword1;
            var model = GetWordsVietEdeSearch(keyword1);
            return View(model);
        }
        public List<Word> GetWordsVietEdeSearch(string keyword1)
        {
            var model = (from a in db.Words
                             //join b in db.Categories
                             //on a.id_cate equals b.Id
                         where a.Viet_Word.Contains(keyword1)
                         select new
                         {
                             Viet_Word = a.Viet_Word ,
                             Cham_Word = a.Cham_Word,
                             Description_Word = a.Description_Word,

                         }).AsEnumerable().Select(x => new Word()
                         {
                             Viet_Word = x.Viet_Word,
                             Cham_Word = x.Cham_Word,
                             Description_Word = x.Description_Word,
                         });

            model.OrderByDescending(x => x.Description_Word);
            return model.ToList();
        }
        public List<string> wordviet(string keyword1)
        {
            return db.Words.Where(x => x.Viet_Word.Contains(keyword1)).Select(x => x.Viet_Word).ToList();
        }
        public JsonResult ListWordViet(string a)
        {
            //List<string> words = db.Words.Where(s => s.Ede.StartsWith(q))

            //    .Select(x => x.Ede).ToList();
            var data = wordviet(a);
            return Json(new
            {
                data = data,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }
    }
}