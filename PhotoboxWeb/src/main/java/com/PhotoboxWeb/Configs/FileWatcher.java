package com.PhotoboxWeb.Configs;

import java.io.IOException;
import java.nio.file.*;

import static com.PhotoboxWeb.PhotoBoothApplication.NewPictureName;

public class FileWatcher {
    private final WatchService watchService;
    private final Path directory;

    public FileWatcher(String directoryPath) throws IOException {
        this.watchService = FileSystems.getDefault().newWatchService();
        this.directory = Paths.get(directoryPath);
        this.directory.register(watchService, StandardWatchEventKinds.ENTRY_CREATE);
    }

    public void watch() {
        try {
            while (true) {
                WatchKey key = watchService.take();

                for (WatchEvent<?> event : key.pollEvents()) {
                    if (event.kind() == StandardWatchEventKinds.ENTRY_CREATE) {
                        Path filePath = (Path) event.context();
                        String newestFile = filePath.toString();
                        if(!newestFile.contains("downscaled")){
                            System.out.println("Newest file: " + newestFile);
                            NewPictureName = newestFile;
                        }
                    }
                }

                key.reset();
            }
        } catch (InterruptedException e) {
            // Handle the interruption or exit gracefully
        }
    }
}

