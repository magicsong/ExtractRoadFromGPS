function LoadingDone() {
    $('#loading').hide(500);
}
var map = new BMap.Map("container", { enableMapClick: false,minZoom:4,maxZoom:18});          // 创建地图实例
// 地图自定义样式
map.setMapStyle({
    styleJson: [
  {
      "featureType": "land",
      "elementType": "geometry",
      "stylers": {
          "color": "#212121"
      }
  },
  {
      "featureType": "building",
      "elementType": "geometry",
      "stylers": {
          "color": "#2b2b2b"
      }
  },
  {
      "featureType": "highway",
      "elementType": "all",
      "stylers": {
          "lightness": -75,
          "saturation": -91
      }
  },
  {
      "featureType": "arterial",
      "elementType": "geometry",
      "stylers": {
          "lightness": -82,
          "saturation": -94
      }
  },
  {
      "featureType": "green",
      "elementType": "geometry",
      "stylers": {
          "color": "#1b1b1b"
      }
  },
  {
      "featureType": "water",
      "elementType": "geometry",
      "stylers": {
          "color": "#181818"
      }
  },
  {
      "featureType": "subway",
      "elementType": "all",
      "stylers": {
          "lightness": -100,
          "saturation": -91
      }
  },
  {
      "featureType": "railway",
      "elementType": "geometry",
      "stylers": {
          "lightness": -84
      }
  },
  {
      "featureType": "all",
      "elementType": "labels.text.stroke",
      "stylers": {
          "color": "#313131"
      }
  },
  {
      "featureType": "all",
      "elementType": "labels",
      "stylers": {
          "color": "#8b8787",
          "lightness": -19,
          "visibility": "off"
      }
  },
  {
      "featureType": "manmade",
      "elementType": "geometry",
      "stylers": {
          "color": "#1b1b1b"
      }
  },
  {
      "featureType": "local",
      "elementType": "geometry",
      "stylers": {
          "lightness": -97,
          "saturation": -100,
          "visibility": "off"
      }
  },
  {
      "featureType": "subway",
      "elementType": "geometry",
      "stylers": {
          "lightness": -76
      }
  },
  {
      "featureType": "railway",
      "elementType": "all",
      "stylers": {
          "lightness": -40,
          "visibility": "off"
      }
  },
  {
      "featureType": "boundary",
      "elementType": "geometry",
      "stylers": {
          "color": "#8b8787",
          "weight": "1",
          "lightness": -29
      }
  }
    ]
});
map.enableScrollWheelZoom(true);
var orginalCenter = new BMap.Point(117.289237, 31.868524);
map.centerAndZoom(orginalCenter, 15);

//添加一些控件
var top_left_control = new BMap.ScaleControl({ anchor: BMAP_ANCHOR_TOP_LEFT });// 左上角，添加比例尺
var top_left_navigation = new BMap.NavigationControl();  //左上角，添加默认缩放平移控件
map.addControl(top_left_control);
map.addControl(top_left_navigation);
var maplayers = [];
var legendPanel = [];
//添加控件完毕
//这里要添加一个专门的GPS轨迹密度层
var SW = new BMap.Point(117.252426, 31.85576);
var NE = new BMap.Point(117.334500, 31.891217);
var groundOverlayOptions = {
    opacity: 0.7,
    displayOnMinLevel: 14,
    displayOnMaxLevel: 18
}
var groundOverlay = new BMap.GroundOverlay(new BMap.Bounds(SW, NE), groundOverlayOptions);

// 设置GroundOverlay的图片地址
groundOverlay.setImageURL('http://magicsong.xyz/wp-content/uploads/2015/11/GPS轨迹密度拉伸.jpg');
//map.addOverlay(groundOverlay);
//TestMode
//AddMapLayer(groundOverlay, "密度图");
//添加一个点击事件
function showInfo(e) {
    //alert(e.point.lng + ", " + e.point.lat);
}
map.addEventListener("click", showInfo);
map.addEventListener("dragend", function () {
    var center = map.getCenter();
    $("#currentLocation").text(center.lng + "," + center.lat);
});
///工具型函数 Begin
function strToJson(str) {
    var json = eval('(' + str + ')');
    return json;
}
function JSON2MapvJSON(x) {
    var old = strToJson(x);
    var result = [];
    for (var i = 0; i < old["count"]; i++) {
        result.push({ geo: old["data"][i], count: 1 });
    }
    return result;
}
///工具型函数 End


function ReturnOriginalPostion() {
    map.setZoom(15);
    map.panTo(orginalCenter);
}
//绘制多条轨迹
function DrawGPSTrajectories(x, color, linewidth) {
    var geodata = JSON2MapvJSON(x);
    var options={
        zIndex: 1,
        mapv: mapv,
        dataType: 'polyline',
        data: geodata,
        drawType: 'simple',
        drawOptions: {
            lineWidth: linewidth,
            strokeStyle: color
        }
    };
    var mapvLayer = new mapv.baiduMapLayer(map, geodata, options);
    AddMapLayer(mapvLayer, "GPS轨迹");
}
function AddMapLayer(layer, name) {
    var index = maplayers.push(layer) - 1;
    var element = $("<a href='#'' class='list-group-item'><input type ='checkbox' checked='checked' id='checkbox" + index + "'>" + name + "</a>");
    $("#layergroup").append(element);
    var currentCheckbox = $("input#checkbox" + index);
    currentCheckbox.iCheck('check', {
        checkboxClass: 'icheckbox_polaris',
        radioClass: 'iradio_polaris',
        increaseArea: '-10' // optional
    });
    currentCheckbox.on('ifUnchecked', function (event) {
        if (layer.canvasLayer)
            layer.canvasLayer.hide();
        else {
            map.removeOverlay(layer);
        }
    });
    currentCheckbox.on('ifChecked', function (event) {
        if (layer.canvasLayer)
            layer.canvasLayer.show();
        else
            map.addOverlay(layer);
    });
    legendPanel.push(element);
}
function AddRoadData() {
    // body...
    var roadData = window.external.GetRoadDataJS();
    DrawGPSTrajectories(roadData, "#FF3399", 4);
}
function AddGPSData() {
    window.external.AddGPSData();
}
function ExportGPSData() {
    window.external.ExportGPSData();
}
function ShowUserTrajectories() {
    var id = $("#inputOfUserID").val();
    if (isNaN(id)) {
        alert("请输入用户ID");
        return;
    }
    var data = window.external.ShowUserTrajectories(id);
    var geodata = eval(data);
    var layer = new Mapv.Layer({
        zIndex: 1,
        mapv: mapv,
        dataType: 'polyline',
        data: geodata,
        drawType: 'simple',
        drawOptions: {
            lineWidth: 2,
            shadowBlur: 20,
            shadowColor: "rgba(250, 255, 0, 1)",
            strokeStyle: "rgba(250, 250, 0, 2)"
        },
        animation: 'time',
        animationOptions: {
            //scope: 60 * 60 * 3,
            size: 8,
            duration: 15000, // 动画时长, 单位毫秒
            fps: 15,         // 每秒帧数
            transition: "linear",
        }
    });
}
function AddGPSDataWithAnimation() {
    var layer = new Mapv.Layer({
        zIndex: 1,
        mapv: mapv,
        dataType: 'polyline',
        data: gpsdata,
        drawType: 'simple',
        drawOptions: {
            lineWidth: 2,
            shadowBlur: 20,
            shadowColor: "rgba(250, 255, 0, 1)",
            strokeStyle: "rgba(250, 250, 0, 2)"
        },
        animation: 'time',
        animationOptions: {
            //scope: 60 * 60 * 3,
            size: 5
        }
    });
}
function DrawGPSDataHeatmap() {
    //window.external.AddGPSData();
    var geodata = eval(window.external.ShowUserTrajectories(-1));
    alert("data is recievied!");
    var options = {
        strokeStyle: 'rgba(50, 50, 255, 0.8)',
        lineWidth: 0.05,
        globalCompositeOperation: 'lighter',
        context: 'webgl',
        draw: 'simple'
    }
    var dataSet = new mapv.DataSet(geodata);
    dataSet.initGeometry();
    var mapvLayer = new mapv.baiduMapLayer(map, dataSet, options);
    AddMapLayer(mapvLayer, "GPS轨迹热力图");
}
var currentGPSTrajectory;
function SimplifyTrajectories() {
    //获取任意轨迹
    //获取简化后轨迹
    //显示两条轨迹
    var traj = window.external.SimplifyGPSTest().split('\n');
    DrawOneLineWithString(traj[0], "yellow");
    DrawOneLineWithString(traj[1], "red");
}
function TestRTree() {
    var traj = window.external.TestRTree().split('\n');
    //1.获取任意一条轨迹 2.获取检索到的道路
    DrawOneLineWithString(traj[0], "yellow");
    for (var i = 1; i < traj.length; i++) {
        if (traj[i] !== "") {
            DrawOneLineWithString(traj[i], "white");
        }
    }
}
function SetCenter(cityname) {
    map.setCurrentCity(cityname);
    map.centerAndZoom(cityname, 15);
}
function DrawRoad(x) {
    x = x.split('\n');
    var geos = [];
    for (var i = 0; i < x.length - 1; i++) {
        xr = x[i].split(',');
        var line = { "geo": new Array(new Array(xr[0], xr[1]), new Array(xr[2], xr[3])), "count": 1 };
        geos.push(line);
    }
    var layer = new Mapv.Layer({
        zIndex: 1,
        mapv: mapv,
        dataType: 'polyline',
        data: geos,
        drawType: 'simple',
        drawOptions: {
            lineWidth: 3,
            strokeStyle: "rgba(30,144,255, 0.5)"
        }
    });
    AddMapLayer(layer, "路网");
    //这里是否要添加其他语句
}
function AllowScrollWheelZoom(enable) {
    map.enableScrollWheelZoom(enable);
}
function AddMassPoints(x, y) {//传入的是两个json,先经度，后纬度
    x = x.split(" ");
    y = y.split(" ");
    var points = [];  // 添加海量点数据
    for (var i = 0; i < x.length; i++) {
        points.push(new BMap.Point(parseFloat(x[i]), parseFloat(y[i])));
    }
    var options = {
        size: BMAP_POINT_SIZE_BIG,
        color: '#d340c3'
    };
    var pointCollection = new BMap.PointCollection(points, options);  // 初始化PointCollection
    pointCollection.addEventListener('click', function (e) {
        window.external.ShowNearestRoad(e.point.lng, e.point.lat);//这里添加查询最近的道路
    });
    map.addOverlay(pointCollection);  // 添加Overlay
}
function DrawPolyline(x) {
    x = x.split(",");
    var pts = [];
    for (var i = 0; i < x.length - 1; i++) {
        pts.push(new BMap.Point(parseFloat(x[i]), parseFloat(x[++i])));
    }
    var polyline = new BMap.Polyline(pts,
            { strokeColor: "red", strokeWeight: 3 });
    map.addOverlay(polyline);
}
function DrawLine(los, las, loe, lae) {
    var polyline = new BMap.Polyline([
            new BMap.Point(los, las),
            new BMap.Point(loe, lae)],
            { strokeColor: "blue", strokeWeight: 3, strokeOpacity: 0.8 });
    map.addOverlay(polyline);
    return polyline;
}
function DrawMarker(pt,url,width,height)
{
    var myIcon = new BMap.Icon(url, new BMap.Size(width, height));
    var marker2 = new BMap.Marker(pt, { icon: myIcon });  // 创建标注
    map.addOverlay(marker2);              // 将标注添加到地图中
}
function DrawOneLineWithBaiduAPI(x, color) {
    if (color === null) {
        color = "yellow";
    }
    var points = [];
    for (var j = 0; j < x["data"].length; j++)
        points.push(new BMap.Point(x["data"][j][0], x["data"][j][1]));
    var polyline = new BMap.Polyline(
        points,
        {
            strokeColor: color,
            strokeWeight: 4
        });
    map.addOverlay(polyline);
    //绘制起点和终点
    DrawMarker(points[0], "Images/startpoint.png", 30, 30);
    DrawMarker(points[points.length-1], "Images/endpoint.png", 30, 30);
    currentPolylines.push(polyline);
    //设置中心
    map.centerAndZoom(points[0], 18);
}
function Show(d) {
    var data = d.split('-');
    var line = window.external.GetOneTrajectory(parseInt(data[0]), parseInt(data[1]));
    //获取到的是一个标准的JSON数据
    line=jQuery.parseJSON(line);
    DrawOneLineWithBaiduAPI(line, "#FF0000");
    //绘制速度曲线
    var ctx = $("#myChart").get(0).getContext("2d");
    var labels = [];
    for (var i = 0; i < line["data"].length; i++)
    {
        labels.push(i + "");
    }
    var chartData = {
        labels:labels,
        datasets: [
            {
                fillColor: "rgba(151,187,205,0.5)",
                strokeColor: "rgba(151,187,205,1)",
                pointColor: "rgba(151,187,205,1)",
                pointStrokeColor: "#fff",
                data: line["Velocity"]
            }
        ]
    };
    var speedchart = new Chart(ctx).Line(chartData, {
        scaleLabel: "<%=value+' km/h'%>"
    });
}
var isScrollable = false;
var currentPolylines = [];
function EnterUserID(event) {
    if (event.keyCode === 13) {
        var inputID = parseInt($("#UserIDInput").val());
        if (isNaN(inputID)) {
            alert("请输入正确的ID");
        }
        else {
            //检索所有轨迹
            $("#UserTraces").html("");
            var data = window.external.GetUserTrajectories(inputID).split(',');
            for (var i = 0; i < data.length; i++) {
                var currentTraceInfo = inputID + "-" + i;
                $("#UserTraces").html($("#UserTraces").html() + "<a href='#' class='list-group-item' onclick=Show('" + currentTraceInfo + "')>轨迹" + i + " <span class='badge'>" + data[i] + "</span></a>");
            }
            if(!isScrollable)
            {
                $("#AllTraces").mCustomScrollbar({
                    theme: "minimal-dark"
                });
                isScrollable = true;
            }
        }
    }
}