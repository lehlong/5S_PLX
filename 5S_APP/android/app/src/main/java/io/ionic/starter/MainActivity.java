package io.ionic.starter;

import com.getcapacitor.BridgeActivity;
import com.google.firebase.messaging.FirebaseMessaging;
import android.os.Bundle;
import android.util.Log;

public class MainActivity extends BridgeActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        // Lấy FCM token khi app khởi động
        FirebaseMessaging.getInstance().getToken()
            .addOnCompleteListener(task -> {
                if (!task.isSuccessful()) {
                    Log.w("FCM", "Lấy token thất bại", task.getException());
                    return;
                }
                String token = task.getResult();
                Log.d("FCM", "Token thiết bị: " + token);
                // TODO: Gửi token này lên server nếu cần
            });
    }
}
