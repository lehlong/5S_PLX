import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { ConfigService } from '../config.service';

@Injectable({ providedIn: 'root' })
export class NetworkSpeedService {

    private baseUrl = environment.apiUrl;
    private refreshTokenInProgress = false;

    constructor(
        private configService: ConfigService
    ) {
        this.configService.apiUrl$.subscribe((url) => {
            this.baseUrl = url;
        });
    }
    async measureSpeed(): Promise<number> {
        const testUrl = this.baseUrl + "/AppEvaluate/SpeedTest";
        const fileSize = 1 * 1024 * 1024; // 1MB

        const startTime = performance.now();
        const response = await fetch(testUrl, { cache: "no-cache" });
        await response.blob();
        const duration = (performance.now() - startTime) / 1000;

        const speedMbps = (fileSize / duration) / 1024 / 1024;
        return Number(speedMbps.toFixed(2));
    }
}