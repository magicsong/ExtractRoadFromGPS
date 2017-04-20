//设置一下MapDiv的高度合理
$("#map").height($(window).height() * 0.85);
mapboxgl.accessToken = 'pk.eyJ1IjoibWFnaWNzb25nIiwiYSI6IkNnNlVfVjAifQ.zP_Kz2PoBDcBqtvb9NF_Dg';
var map = new mapboxgl.Map({
    container: 'map',
    style: 'mapbox://styles/mapbox/light-v9',
    zoom: 13.7,
    center:[117.285,31.863]
});
map.addControl(new mapboxgl.NavigationControl());
map.addControl(new mapboxgl.FullscreenControl());