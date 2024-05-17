package com.PhotoboxWeb.Pictures;

import com.PhotoboxWeb.PhotoBoothApplication;
import org.springframework.core.io.FileSystemResource;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;
import org.springframework.stereotype.Controller;

import java.io.File;
import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;
import java.util.ArrayList;
import java.util.List;

import static com.PhotoboxWeb.PhotoBoothApplication.PhotoBoxDirectory;
import static com.PhotoboxWeb.PhotoBoothApplication.ipAddress;


/**
 * This controller is responsible for everything that has to do with the pictures.
 * It is responsible for showing the pictures on the website and for downloading the pictures.
 * It is also responsible for deleting the pictures and shutting down the server.
 *
 */
@Controller
@RequestMapping("/Pictures")
public class UserPicturesDownloadController {

    private final String PictureDirectory = Paths.get("Images").toString();

    /**
     * this Endpoint is responsible for listing all the prictures that are in the directory and display them to the user
     * @return the build HTML containing all pictures
     */
    @GetMapping("/old")
    private ResponseEntity<String> GetPictures()
    {

        File[] imageFiles = getImageFilesFromDirectory(PhotoBoothApplication.PhotoBoxDirectory);

        if (imageFiles == null || imageFiles.length == 0) {
            return ResponseEntity.ok("No pictures found");
        }

        StringBuilder responseHTML = new StringBuilder();

        responseHTML.append("<!Doctype html>");
        responseHTML.append("<html>");
        responseHTML.append("<div style=\"background-color: orange;\">");

        for (File imageFile : imageFiles) {
            String imageName = imageFile.getName();

            if(!imageName.contains("downscaled")) {continue;}

            String imagePath = "/Images/" + imageName;  // Adjust the path as needed

            // Wrap the img tag with an anchor tag to create a clickable link
            responseHTML.append("<a href=\"/Pictures/showImage/"+imageName).append("\">");
            responseHTML.append("<img src=\"").append(imagePath).append("\" alt=\"").append(imageName)
                    .append("\" style=\"max-width: 100%; height: auto;padding-top: 10px; padding-bottom: 10px;\">");
            responseHTML.append("</a>");
        }

        responseHTML.append("</div>");
        responseHTML.append("</html>");

        return ResponseEntity.ok(responseHTML.toString());
    }

    /**
     * This endpoint is just a
     * @param model
     * @return
     */
    @GetMapping("/")
    public String GetPicturesTest(Model model)
    {

        File[] imageFiles = getImageFilesFromDirectory(PhotoBoxDirectory);

        if (imageFiles == null || imageFiles.length == 0) {
            return "No pictures found";
        }

        List<String> imageNames = new ArrayList<>();

        for(int i = imageFiles.length - 1; i >= 0; i--)
        {
            File imageFile = imageFiles[i];

            String imageName = imageFile.getName();

            if(!imageName.contains("downscaled")) {continue;}

            imageNames.add(imageName);
        }

        model.addAttribute("imageNames", imageNames);

        model.addAttribute("ipAddress", ipAddress);

        return "pictureGallery";
    }

    /**
     * Displays the image in fullscreen and gives the user the buttons to download and reprint the image
     * @param imageName the name of the image that will be displayed to the user
     * @param model
     * @return the Thymeleaf template showImage
     * @throws IOException
     */
    @GetMapping("/showImage/{imageName}")
    public String showImage(@PathVariable String imageName, Model model) throws IOException {

        imageName = imageName.replace("downscaled" , "");

        String imagePath = "/Images/" + imageName;  // Adjust the path as needed

        model.addAttribute("imagePath", imagePath);
        model.addAttribute("imageName", imageName);
        model.addAttribute("ipAddress", ipAddress);


        return "showImage"; // Return the name of the Thymeleaf template
    }


    /**
     * This method is responsible for downloading the pictures to the client.
     *
     * @param imageName is the name of the picture that is going to be downloaded.
     * @return ResponseEntity<FileSystemResource> is the response that is going to be sent to the client.
     * @throws IOException
     */
    @GetMapping("/Download/{imageName}")
    @ResponseBody
    public ResponseEntity<FileSystemResource> serveImage(@PathVariable String imageName) throws IOException {

        imageName = imageName.replace("downscaled" , "");

        Path imagePath = Paths.get(PhotoBoxDirectory +"\\"+ imageName);

        File fileToDownload = new File(imagePath.toString());

        if (!fileToDownload.exists()) {
            return ResponseEntity.notFound().build();
        }

        // Suggest a filename for download.
        String suggestedFilename = imageName;

        // Set the Content-Disposition header to suggest the filename.
        HttpHeaders headers = new HttpHeaders();
        headers.add(HttpHeaders.CONTENT_DISPOSITION, "attachment; filename=" + suggestedFilename);

        // Set the content type based on the file's MIME type.
        MediaType mediaType = MediaType.APPLICATION_OCTET_STREAM;
        try {
            Path path = fileToDownload.toPath();
            mediaType = MediaType.valueOf(Files.probeContentType(path));
        } catch (IOException e) {
            e.printStackTrace();
        }

        // Create a FileSystemResource to represent the file.
        FileSystemResource resource = new FileSystemResource(fileToDownload);

        // Return the file as a ResponseEntity with appropriate headers.
        return ResponseEntity.ok()
                .headers(headers)
                .contentLength(fileToDownload.length())
                .contentType(mediaType)
                .body(resource);
    }

    private File[] getImageFilesFromDirectory(String directoryPath) {
        Path imagePath = Paths.get(directoryPath);
        File directory = imagePath.toFile();

        if (directory.exists() && directory.isDirectory()) {
            return directory.listFiles((dir, name) -> {
                String lowercaseName = name.toLowerCase();
                return lowercaseName.endsWith(".jpg") || lowercaseName.endsWith(".jpeg");
            });
        } else {
            return null;
        }
    }


}
