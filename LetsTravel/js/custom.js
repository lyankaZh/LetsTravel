// Offset for Site Navigation
$('#siteNav').affix({
	offset: {
		top: 100
	}
})


    
    $(document).ready(function () {
        $('li.active').removeClass('active');
        $('a[href="' + location.pathname + '"]').closest('li').addClass('active');
    });