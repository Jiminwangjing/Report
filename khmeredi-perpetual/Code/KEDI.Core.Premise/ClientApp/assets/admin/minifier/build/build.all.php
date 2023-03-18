<?php
	include_once("vendor/core.php");

	$to = "../../../../wwwroot/AdminLayout/bundle";
	
	$css_var = "variables.core.scss";
	
	$ext = array(
	    "admin", "package", "base", "core", "dialog", "calculator", "search", "combobox", "viewtable", "FormatNumber"
	);
	
	build();
?>