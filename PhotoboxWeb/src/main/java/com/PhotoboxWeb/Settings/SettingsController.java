package com.PhotoboxWeb.Settings;

import com.PhotoboxWeb.DTOs.SettingsDTO;
import com.PhotoboxWeb.PhotoBoothApplication;
import com.fasterxml.jackson.databind.ObjectMapper;
import org.json.simple.JSONObject;
import org.json.simple.parser.JSONParser;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.*;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.FileWriter;
import java.io.IOException;
import java.util.stream.Collectors;

@Controller
@RequestMapping("/Settings")
public class SettingsController {


    JSONParser jsonParser = new JSONParser();

    private ObjectMapper objectMapper = new ObjectMapper();

    @GetMapping("/")
    public String getSettings(Model model) throws IOException {

        try(FileReader reader = new FileReader(PhotoBoothApplication.SettingsPath);)
        {
            Object obj = jsonParser.parse(reader);

            JSONObject settings = (JSONObject) obj;

            SettingsDTO settingsDTO = loadSettings();

            System.out.println(settingsDTO);

            model.addAttribute("settingsDTO", settingsDTO);

            return "Settings"; // The name of your Thymeleaf HTML file
        }
        catch(Exception ex)
        {
            System.out.print("Error: " + ex);
        }

        return "ok";
    }

    @PostMapping("/Save")
    public String Save(@ModelAttribute SettingsDTO settingsDTO) {

        try
        {
            saveSettings(settingsDTO);
        }
        catch(Exception ex)
        {
            System.out.println("Error Happened: " + ex);
        }

        return "redirect:/Settings/";
    }

    public static String read(String filePath) throws IOException {
        StringBuilder jsonData = new StringBuilder();

        try (BufferedReader reader = new BufferedReader(new FileReader(filePath))) {
            String line;
            while ((line = reader.readLine()) != null) {
                jsonData.append(line);
            }
        }

        return jsonData.toString();
    }

    public SettingsDTO loadSettings() throws IOException {
        // Read the JSON data from the file
        String jsonSettings;
        try (BufferedReader reader = new BufferedReader(new FileReader(PhotoBoothApplication.SettingsPath))) {
            jsonSettings = reader.lines().collect(Collectors.joining());
        }

        // Deserialize the JSON data into a SettingsDTO object
        SettingsDTO settingsDTO = objectMapper.readValue(jsonSettings, SettingsDTO.class);

        return settingsDTO;
    }

    public void saveSettings(SettingsDTO settingsDTO) throws IOException {
        // Serialize the SettingsDTO to a JSON string
        String jsonSettings = objectMapper.writeValueAsString(settingsDTO);

        // Write the JSON string to the file
        try (FileWriter fileWriter = new FileWriter(PhotoBoothApplication.SettingsPath)) {
            fileWriter.write(jsonSettings);
        }
    }




}



