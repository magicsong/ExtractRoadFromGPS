(function () {
    var elements = document.getElementsByClassName("comment_txt");
    var data = "";
    //第一行就是下一页的网址
    var link = document.getElementsByClassName("page next S_txt1 S_line1");
    link = link[0];
    if (typeof link == "undefined")
        return data;
    //http://s.weibo.com/weibo/%25E5%2585%25AC%25E5%2585%25B1%25E8%2587%25AA%25E8%25A1%258C%25E8%25BD%25A6&page=2
    var href = "http://s.weibo.com" + link.getAttribute("href");
    data += href + "\n";
    for (item of elements) {
        var str = item.textContent.replace('\n', '');
        data += str + '\n';
    }
    return data;
})();
