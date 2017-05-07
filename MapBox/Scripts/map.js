//设置一下MapDiv的高度合理
//$("#map").height($(window).height() * 0.85);
mapboxgl.accessToken = 'pk.eyJ1IjoibWFnaWNzb25nIiwiYSI6IkNnNlVfVjAifQ.zP_Kz2PoBDcBqtvb9NF_Dg';
var map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/magicsong/cj1t2vdnv002w2rnzie8q6ize',
    zoom: 13.7,
    center: [117.2788, 31.8664]
});
map.addControl(new mapboxgl.NavigationControl());
//map.addControl(new mapboxgl.FullscreenControl());
map.on('mousemove', function (e) {
    $("#currentX").html(e.lngLat.lng);
    $("#currentY").html(e.lngLat.lat);
});
var roadID = "road";
var GPSID = "GPS";
function LoadingAnimate(id, name) {
    $("#loadSVG").show();
    var listener = function (e) {
        if (e.isSourceLoaded && e.tile !== undefined) {
            map.off("sourcedata", listener);
            AddLayerToLegend(id, name);
            $("#loadSVG").hide();
        }
    };
    map.on("sourcedata", listener);
}
function AddRoad() {
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
    AddLayerToLegend(roadID, "路网");
}
function AddGPS() {
    map.addSource("GPSSource", {
        type: 'geojson',
        data: 'http://localhost:1228/Home/GetGPS'
    });
    map.addLayer(
        {
            "id": GPSID,
            type: "line",
            source: 'GPSSource',
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
    LoadingAnimate(GPSID, "GPS轨迹");
}
function AddOPoints1() {
    map.addSource("startpoints", {
        type: "geojson",
        // Point to GeoJSON data. This example visualizes all M1.0+ earthquakes
        // from 12/22/15 to 1/21/16 as logged by USGS' Earthquake hazards program.
        data: 'http://localhost:1228/GetData/GetStartPoints',
        cluster: true,
        clusterMaxZoom: 20, // Max zoom to cluster points on
        clusterRadius: 20 // Use small cluster radius for the heatmap look
    });
    var layers = [
        [30, '#ffffd4'],
        [100, '#fed98e'],
        [300, '#fe9929'],
        [500, '#d95f0e']
        //[1000, '#993404']
    ];

    layers.forEach(function (layer, i) {
        map.addLayer({
            "id": "cluster-" + i,
            "type": "circle",
            "source": "startpoints",
            "paint": {
                "circle-color": layer[1],
                "circle-radius": 70,
                "circle-blur": 0.5 // blur the circles to get a heatmap look
            },
            "filter": i === layers.length - 1 ?
                [">=", "point_count", layer[0]] :
                ["all",
                    [">=", "point_count", layer[0]],
                    ["<", "point_count", layers[i + 1][0]]]
        });
    });

    //map.addLayer({
    //    "id": "unclustered-points",
    //    "type": "circle",
    //    "source": "startpoints",
    //    "paint": {
    //        "circle-color": 'rgba(255,255,255,0.5)',
    //        "circle-radius": 10,
    //        "circle-blur": 0.5
    //    },
    //    "filter": ["!=", "cluster", true]
    //});


}
function AddOPoints() {
    map.addSource("startpoints", {
        type: "geojson",
        data: 'http://localhost:1228/GetData/GetStartPoints'
    });
    map.addLayer({
        "id": "startpoints",
        "type": "circle",
        "source": "startpoints",
        "paint": {
            "circle-color": 'rgba(52, 152, 219,0.1)',
            "circle-radius": 3,
            "circle-blur": 0
        }
    });
    LoadingAnimate("startpoints", "起点图");
}
function AddDPoints() {
    map.addSource("endpoints", {
        type: "geojson",
        data: 'http://localhost:1228/GetData/GetEndPoints'
    });
    map.addLayer({
        "id": "endpoints",
        "type": "circle",
        "source": "endpoints",
        "paint": {
            "circle-color": 'rgba(231, 76, 60,0.1)',
            "circle-radius": 3,
            "circle-blur": 0
        }
    });
    LoadingAnimate("endpoints", "终点图");
}
function AddLayerToLegend(id, name) {
    $("#layergroup").append("<li class='list-group-item'>"+ name +
        "<div class='material-switch pull-right' >"+
            "<input id='"+id+"' type='checkbox' checked/>"+
            "<label for='" + id +"' class='label-primary'></label>"+
         "</div >" +
        "</li >");
    $("#" + id).change((event) => {
        console.log(id);
        if ($("#" + id).is(':checked'))
            map.setLayoutProperty(id, "visibility", "visible");
        else
            map.setLayoutProperty(id, "visibility", "none");
    });
}
function AddJsonSymbolPoints(sourceID, sourceURL, layerID, iconURL, legendName, iconsize) {
    map.loadImage(iconURL, (error, image) => {
        if (error)
            throw error;
        map.addImage(layerID + 'icon', image);
        map.addSource(sourceID, {
            type: "geojson",
            data: sourceURL
        });
        map.addLayer({
            "id": layerID,
            "type": "symbol",
            "source": sourceID,
            "paint": {
                "icon-color": "#FFFF99"
            },
            "layout": {
                "icon-image": layerID + 'icon',
                "icon-size": iconsize
            }
        });
        AddLayerToLegend(layerID, legendName);
    });
}
function AddJsonCirclePoints(sourceID, sourceURL, layerID, color, legendName, size) {
    map.addSource(sourceID, {
        type: "geojson",
        data: sourceURL
    });
    map.addLayer({
        "id": layerID,
        "type": "circle",
        "source": sourceID,
        "paint": {
            "circle-radius": size,
            "circle-color": color
        },
    });
    AddLayerToLegend(layerID, legendName);
}
function AddCentroidPoints() {
    AddJsonSymbolPoints('CentroidPoints', 'http://localhost:1228/GetData/GetJSON?filename=CentroidPoints', "Centroid", 'http://localhost:1228/images/embassy.png', "预设中心点", 0.5);
}
function AddBusStop() {
    AddJsonSymbolPoints("BusStop", 'http://localhost:1228/GetData/GetJSON?filename=BusStop', "bus", 'http://localhost:1228/images/bus.png', "公交车站", 1);
}
function AddNewCentroidPoints() {
    AddJsonCirclePoints('newCentroidPoints', 'http://localhost:1228/GetData/GetJSON?filename=newCentroid', "newCentroid", "#66B2FF", "规划后中心点", 6);
}