<?php
	include_once("vendor/core.php");

	$to = "../output/kbms";
	
	$css_var = "variables.core.scss";
	
	$ext = array(
		"package","dialog", "base", "viewtable", "kbms"
	);
	
	build();