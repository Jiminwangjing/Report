<?php
	include_once("vendor/core.php");

	$to = "../output/krms";
	
	$css_var = "variables.core.scss";
	
	$ext = array(
		"package","dialog", "base", "viewtable", "krms"
	);
	
	build();