/**
 * @file MapV框架下的数据可视化

 * @author MagicSong(xuetaomagicsong@gmail.com)
 */

/**
*/

var map = new BMap.Map("container", { enableMapClick: false, minZoom: 4, maxZoom: 18 });          // 创建地图实例
// 地图自定义样式
map.setMapStyle({
    styleJson: [
        {
            "featureType": "all",
            "elementType": "all",
            "stylers": {
                "lightness": 61,
                "saturation": -100
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

/**
 * 添加一个图层到图层面板
 * 
 * @param {mapv.baiduMapLayer} layer 表示传入的图层
 * @param {string} name 图层的名字 
 */
function AddMapLayer(layer, name) {
    var index = maplayers.push(layer) - 1;
    var id = "checkbox" + index;
    $("#layergroup").html($("#layergroup").html() + "<a href='#'' class='list-group-item'><input type ='checkbox' checked='checked' id='"+id + "'>" + name + "</a>");
    $('#'+id).iCheck({
        checkboxClass: 'icheckbox_polaris',
        radioClass: 'iradio_polaris',
        increaseArea: '-10%' // optional
    }).on('ifUnchecked', function (event) {
        layer.hide();
    }).on('ifChecked', function (event) {
        layer.show();
    });
}
/**
 * 用于绘制多条线数据
 * 
 * @param {string} x 传入的json数据字符串
 * @param {string} color 传入的标准CSS的颜色
 * @param {int} linewidth 线宽度
 * @param {string} name 该图层的名字
 * @param {bool} isWebGL 是否使用WebGL渲染大数据量，默认为false
 */
function DrawMultiLines(x, color, linewidth,name,isWebGL) {
    var geodata = new mapv.DataSet(JSON.parse(x));
    //如果数据量大的话需要有一些优化
    geodata.initGeometry();
    var options={
        strokeStyle:color,
        linewidth:linewidth,
        shadowColor: 'rgba(53,57,255,0.5)',
        globalCompositeOperation: "lighter",
        shadowBlur: 3,
        draw:'simple'
    };
    var mapvLayer = new mapv.baiduMapLayer(map, geodata, options);
    AddMapLayer(mapvLayer, name);
}

/////////////////////////////HTML用函数/////////////////////////////////////////

/**
 * 隐藏刚开始的Loading界面
 * 
 */
function LoadingDone() {
    $('#loading').hide(500);
}

/**
 * 添加路网数据
 * 
 */
function AddRoadData() {
    var roadData = window.external.GetRoadDataJS();
    DrawMultiLines(roadData, "rgba(255, 250, 250, 0.8)", 5, "路网");
}
function AddGPSData() {
    var gpsData = window.external.GetGPSDataJS();
    DrawMultiLines(gpsData, "rgba(250, 50, 50, 0.3)", 0.1, "GPS轨迹");
}
function ShowUserTrajectories() {
    var id = $("#inputOfUserID").val();
    if (isNaN(id)) {
        alert("请输入用户ID");
        return;
    }
    var data = window.external.ShowUserTrajectories(id);
}
