package com.PhotoboxWeb;

import com.PhotoboxWeb.Configs.FileWatcher;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;

import java.io.File;
import java.io.IOException;
import java.net.InetAddress;
import java.net.UnknownHostException;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.Arrays;
import java.util.Comparator;

@SpringBootApplication
public class PhotoBoothApplication {

	public static final String PhotoBoxDirectory = System.getenv("USERPROFILE") + "\\Pictures\\PhotoBox\\Static";

	public static final Path resourceDirectory = Paths.get("Images");

	public static String ipAddress = "";

	public static String NewPictureName = "";

	//public static final String SettingsPath = "C:\\Users\\Robin\\Documents\\GitHub\\CanonPhotoBooth\\CanonPhotoBooth\\Resources\\config.json";

	public static final String SettingsPath = "Resources\\config.json";

	public static void main(String[] args) {
		SpringApplication.run(PhotoBoothApplication.class, args);

		try {
			ipAddress = getIPAddress();
		} catch (UnknownHostException e) {
			e.printStackTrace();
		}

		System.out.println("PhotoBoxDirectory: " + PhotoBoxDirectory);

		NewPictureName = findNewestFileInDirectory(PhotoBoxDirectory).replace("downscaled", "");

		try {
			FileWatcher fileWatcher = new FileWatcher(PhotoBoxDirectory);
			fileWatcher.watch();
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public static String getIPAddress() throws UnknownHostException {
		InetAddress localhost = InetAddress.getLocalHost();
		return localhost.getHostAddress();
	}

	public static String findNewestFileInDirectory(String directoryPath) {
		File directory = new File(directoryPath);

		if (directory.exists() && directory.isDirectory()) {
			File[] files = directory.listFiles();

			if (files != null && files.length > 0) {
				Arrays.sort(files, Comparator.comparingLong(File::lastModified).reversed());
				return files[0].getName();
			}
		}

		return null;
	}
}
