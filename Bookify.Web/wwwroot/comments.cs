namespace Bookify.Web.wwwroot
{
    public class comments
    {
        //ازاي تقدر تضيف الداتا تيبلز عندك ف البروجكت
        //https://datatables.net/ هتخش ع موقع 
        //cdn دا عشان لو عاوز تستخدمه ك
        ////cdn.datatables.net/2.3.8/css/dataTables.dataTables.min.css --> هترمي دا ف اللاي اوت 
        /////cdn.datatables.net/2.3.8/js/dataTables.min.js --> وارمي دا كمان 
        ///let table = new DataTable('#myTable'); -->ودا جوا الجي كويري
        ///
        //طيب لو عاوز تنزله 
        //~/libهتروح ع ال 
        //بعدين تعمل انستول بقا  datatables كليك يمين ادد بعدين كلاينت سايد لايبرالي بعد كدا تسيرش عن   
        //وترمية جوا اللاي اوت css and js بتاع ال .min بعد كدا بتروح ترمي فايل اللي اخرة 
        //تكتب السطر دا مثلا لو انت عامل الجي كويري ف صفحة الاندكس او علي حسب بقا انت بتعرض التيبل ف انهي صفحة <script>$('table').DataTable() </script> بعد كدا بتروح ف الجي كويري جوا ال  
        //بس كدا يمعلم التيبلز اللي عندك هيطبق عليها خواص الداتا تيبل 
        //datatables.net-buttons-bs5@2.2.3 وكمان pdfmake وتحمل كمان باكدج  jszip وحمل كمان باكدج ال   datatables-buttons وتحمل   ~/lib هتروح الاول تنزل بردوا باكدج ف ال  pdf or excel or print طيب عشان تقدر تضيف ميزة انه يقدر يعمل اكسبورت للتيبل ك 
        //هتروح ف اللاي اوت ترميهم ب الترتيب دا 
        //<script src="~/lib/datatables-buttons/js/dataTables.buttons.min.js"></script> 
        //<script src = "~/lib/datatables.net-buttons-bs5/buttons.bootstrap5.min.js" ></ script >
        //<script src="~/lib/jszip/jszip.min.js"></script>
        //<script src="~/lib/pdfmake/pdfmake.min.js"></script>
        //<script src="~/lib/pdfmake/vfs_fonts.min.js"></script>
        //<script src="~/lib/datatables-buttons/js/buttons.html5.min.js"></script>
        //<script src="~/lib/datatables-buttons/js/buttons.print.min.js"></script>

        //ارمي دا  css وفوق ف ال 
        //<link href="~/lib/datatables.net-buttons-bs5/buttons.bootstrap5.min.css" rel="stylesheet" />


        //ناقص بقا تضيف جوا الجي كويري الكود دا 
        //$('table').DataTable({
        //    dom: 'Bfrtip',
        //buttons: [
        //    'copy', 'csv', 'excel', 'pdf', 'print'
        //]
        //});

        //ولكن خلي بالك من حاجه انك عشان بس تطبق الداتا تيبل عملت رنرد للملفات دي كلها ف اللاي اوت يعني الملفات دي هترندر مع كل صفحة ف الموقع واصلا فيه صفحات في غني عن املفات دي لان ببساطة صفحات مفهاش تيبلز 
        //@await RenderSectionAsync("Customscripts", required: false) ف الافضل نعملها رندر سيشكن ونعمله ان اوبشنال يعني مش ريكوايرد 
        //@await RenderSectionAsync("Customscripts", required: false) خد بقا الملفات دي كلها كات من الاندكس ارميها ف البارشيل فيو وحط مكان السطر اللي شلتها حط السطر دا عشان يتعمله رندر   _DataTablesJs وتاخد الملفات دي كلها ترميها ف بارشل فيو سمية مثلا

        //هتروح ف الاندكس ترندر السكشن سكريبت اللي نت عملته 
        //@section CustomScripts
        //{
        //    <partial name="_DataTablesJS" />
        //    <script>
        //        //$('table').DataTable({
        //        //    dom: 'Bfrtip',
        //        //buttons: [
        //        //    'copy', 'csv', 'excel', 'pdf', 'print'
        //        //]
        //        //});

        //    </script>
        //};

        //وبرود ممكن تاخد فايلس ال سي اس اس ترميها ف بارشل فيو وتعمل مكانها رندر سيشمن وتخليه مش ريكواريد وتروح ع الاندكس ترندره ك الاتي 
        //@section Styles
        //{
        //    <partial name = "_DataTablesCSS" />
        //}

}
}
