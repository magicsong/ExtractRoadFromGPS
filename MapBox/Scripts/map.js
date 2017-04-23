//设置一下MapDiv的高度合理
//$("#map").height($(window).height() * 0.85);
mapboxgl.accessToken = 'pk.eyJ1IjoibWFnaWNzb25nIiwiYSI6IkNnNlVfVjAifQ.zP_Kz2PoBDcBqtvb9NF_Dg';
var map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/magicsong/cj1t2vdnv002w2rnzie8q6ize',
    zoom: 13.7,
    center:[117.2788,31.8664]
});
map.addControl(new mapboxgl.NavigationControl());
//map.addControl(new mapboxgl.FullscreenControl());
map.on('mousemove', function (e) {
    $("#currentX").html(e.lngLat.lng);
    $("#currentY").html(e.lngLat.lat);
});
var roadID = "road";
var GPSID = "GPS";
function AddRoad()
{
    map.addLayer({
        "id": roadID,
        "type": "line",
        "source": {
            type: 'vector',
            url: 'mapbox://magicsong.11a48f33'
        },
        "source-layer": "road",
        "layout": {
            "line-join": "round",
            "line-cap": "round"
        },
        "paint": {
            "line-color": "#ff69b4",
            "line-width": 2
        }
    });
    AddLayerToLegend(roadID,"路网");
}
function AddGPS()
{
    map.addSource("GPSSource", {
        type: 'geojson',
        data:'http://localhost:1228/Home/GetGPS'
    });
    map.addLayer(
        {
            "id": GPSID,
            type: "line",
            source:'GPSSource' ,
            "layout": {
                "line-join": "round",
                "line-cap": "round"
            },
            "paint": {
                "line-color": "rgba(0,191,255,0.1)",
                "line-width": 1
            }
        }
    );
    $("#loadSVG").show();
    var listener = function (e) {
        if (e.isSourceLoaded && e.tile != undefined) {
            map.off("sourcedata", listener);
            AddLayerToLegend(GPSID, "GPS轨迹");
            $("#loadSVG").hide();
        }
    };
    map.on("sourcedata", listener);
    
}
function AddLayerToLegend(id,name)
{
    $("#layergroup").html($("#layergroup").html() + "<li class='list-group-item'><input type ='checkbox' checked='checked' id='" + id + "'>" + "&nbsp;&nbsp;&nbsp;"+name + "</li>");
    $('#' + id).iCheck({
        checkboxClass: 'icheckbox_square-blue',
        radioClass: 'iradio_square-blue',
        increaseArea: '20%' // optional
    }).on('ifUnchecked', function (event) {
        map.setLayoutProperty(id, 'visibility', 'none');
    }).on('ifChecked', function (event) {
        map.setLayoutProperty(id, 'visibility', 'visible');
    });
}