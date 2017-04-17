/**
 * @file MapV框架下的数据可视化

 * @author MagicSong(xuetaomagicsong@gmail.com)
 */

/**
*/

/**
 * 用于绘制多条线数据
 * 
 * @param {string} x 传入的json数据字符串
 * @param {string} color 传入的标准CSS的颜色
 * @param {int} linewidth 线宽度
 */
function DrawMultiLines(x, color, linewidth) {
    var data = JSON.parse(x);
    var geodata = new mapv.DataSet(data);
    var mapvLayer = new mapv.baiduMapLayer(map, geodata, options);
    AddMapLayer(mapvLayer, "GPS轨迹");
}