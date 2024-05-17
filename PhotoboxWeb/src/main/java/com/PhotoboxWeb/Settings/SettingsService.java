//package com.PhotoBoothRestApi.Settings;
//
//import com.PhotoBoothRestApi.DTOs.SettingsDTO;
//import com.fasterxml.jackson.databind.JsonNode;
//import com.fasterxml.jackson.databind.ObjectMapper;
//import org.springframework.core.io.Resource;
//import org.springframework.core.io.ResourceLoader;
//import org.springframework.stereotype.Service;
//import java.io.IOException;
//
////@Service
//public class SettingsService {
//    //    private final ResourceLoader resourceLoader;
////    private final ObjectMapper objectMapper;
////    private final String configFilePath;
////
////
////    public SettingsService(ResourceLoader resourceLoader, ObjectMapper objectMapper, String configFilePath) {
////        this.resourceLoader = resourceLoader;
////        this.objectMapper = objectMapper;
////        this.configFilePath = "C:\\Users\\Robin\\Documents\\GitHub\\CanonPhotoBooth\\CanonPhotoBooth\\Resources\\config.json";
////    }
////
////    public SettingsDTO loadSettingsFromJson() throws IOException {
////        Resource resource = resourceLoader.getResource("file:" + configFilePath);
////        return objectMapper.readValue(resource.getInputStream(), SettingsDTO.class);
////    }
//}
