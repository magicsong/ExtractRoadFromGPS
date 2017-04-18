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

/**
 * 添加一个图层到图层面板
 * 
 * @param {mapv.baiduMapLayer} layer 表示传入的图层
 * @param {string} name 图层的名字 
 */
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
        layer.hide();
    });
    currentCheckbox.on('ifChecked', function (event) {
      layer.show();
    });
    legendPanel.push(element);
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
    DrawMultiLines(roadData, "rgba(255, 250, 250, 0.2)", 5, "路网");
}