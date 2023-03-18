<?php
	include_once("vendor/core.php");

	$to = "../../../../wwwroot/pos/output";
	
	$css_var = "variables.core.scss";
	
	$ext = array(
		"package", "base","dialog", "combobox", "search", "viewtable", "carousel", "dotcontainer"
	);
	
	build();