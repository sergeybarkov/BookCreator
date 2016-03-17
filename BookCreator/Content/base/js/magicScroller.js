var smFlag = true;
getCurrentPagePos();
function $_GET(type){
if(location.href.match(type)){
return location.href.split(type+'=')[1].split('&')[0];
}}
if ($_GET('scrollTo'))
	document.body.scrollTop = $_GET('scrollTo');
function showScrollMarker(){
if (!smFlag) return;
			psize = getPageSize();
			h = psize[3];
			if(currentPagePos - document.body.scrollTop < 0)
				pos = h + currentPagePos- 20;
			else
				pos = currentPagePos- 20;
			$("#sm").css('top', pos);
			$("#sm").show();
			smFlag = false;			
			setTimeout('hideScrollMarker()', 2000);
	}
function hideScrollMarker(){
	if (smFlag) return;
	$("#sm").fadeOut("slow");
	smFlag = true;
}	
function getCurrentPagePos(){
	if (smFlag){
	currentPagePos = document.body.scrollTop;
	}
		setTimeout('getCurrentPagePos()', 10);
}
function  getPageSize(){
			var xScroll, yScroll;

			if (window.innerHeight && window.scrollMaxY) {
				xScroll = document.body.scrollWidth;
				yScroll = window.innerHeight + window.scrollMaxY;
			} else if (document.body.scrollHeight > document.body.offsetHeight){ // all but Explorer Mac
				xScroll = document.body.scrollWidth;
				yScroll = document.body.scrollHeight;
			} else if (document.documentElement && document.documentElement.scrollHeight > document.documentElement.offsetHeight){ // Explorer 6 strict mode
				xScroll = document.documentElement.scrollWidth;
				yScroll = document.documentElement.scrollHeight;
			} else { // Explorer Mac...would also work in Mozilla and Safari
				xScroll = document.body.offsetWidth;
				yScroll = document.body.offsetHeight;
			}

			var windowWidth, windowHeight;
			if (self.innerHeight) { // all except Explorer
				windowWidth = self.innerWidth;
				windowHeight = self.innerHeight;
			} else if (document.documentElement && document.documentElement.clientHeight) { // Explorer 6 Strict Mode
				windowWidth = document.documentElement.clientWidth;
				windowHeight = document.documentElement.clientHeight;
			} else if (document.body) { // other Explorers
				windowWidth = document.body.clientWidth;
				windowHeight = document.body.clientHeight;
			}

			// for small pages with total height less then height of the viewport
			if(yScroll < windowHeight){
				pageHeight = windowHeight;
			} else {
				pageHeight = yScroll;
			}

			// for small pages with total width less then width of the viewport
			if(xScroll < windowWidth){
				pageWidth = windowWidth;
			} else {
				pageWidth = xScroll;
			}

			return [pageWidth,pageHeight,windowWidth,windowHeight];
}