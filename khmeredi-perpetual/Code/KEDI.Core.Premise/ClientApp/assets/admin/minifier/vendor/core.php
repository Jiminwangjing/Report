<?php
    include('autoload.php');
    
	function build() {
		global $ext;
		global $to;
		
		if (!file_exists($to)) {
			mkdir($to, true);
		}
		list($js, $css) = collect_files($ext);
		build_js($js, $to);
		build_scss($css, $to);
	}
	
	function collect_files($exts) {
		global $css_var;
		
		$ext_path = "modules/";
		$css = array($ext_path . $css_var);
		$js = array();
		
		echo "Scanning the file...\n";
		foreach($exts as $x) {
			$x_path = $ext_path . $x;
			$files = array_diff(scandir($x_path), array('.', '..'));
			
			foreach($files as $f) {
				
				$file_ext = pathinfo($f, PATHINFO_EXTENSION);
				switch($file_ext) {
					case "scss":
						$css[] = $x_path . "/" . $f;
						break;
					case "js":
						$js[] = $x_path . "/" . $f;
				}
				
				echo "[x] " . $x_path . "/" . $f . "\n";
			}
		}
		
		return array($js, $css);
	}
	
	function build_js($files, $to) {
		echo "Building Javascript files\n";
		
		$result = "";
		foreach($files as $file) {
			echo "[x] Reading " . $file . "\n";
			$tmp = file_get_contents($file);
			$result .= $tmp;
		}
        // $result = \JShrink\Minifier::minify($result);
        $result = \JShrink\Minifier::minify($result, array('flaggedComments' => false));
		echo "[x] Saving the combined Javascript\n";
		file_put_contents($to . "/core.js", $result);
	}
	
	function build_scss($files, $to) {
		$tmp_file = "build/tmp.scss";
		
		echo "Creating the SCSS file.";
		$css_imports = "";
		foreach($files as $file) {
			$css_imports .= '@import "../' . $file . '";';
		}
		file_put_contents($tmp_file, $css_imports);
		exec("scss {$tmp_file} > {$to}/core.css --style compressed");
	}
