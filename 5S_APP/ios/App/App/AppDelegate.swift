import UIKit
import Capacitor
import CoreLocation
import CapacitorCamera 

@UIApplicationMain
class AppDelegate: UIResponder, UIApplicationDelegate, CLLocationManagerDelegate {

    var window: UIWindow?
    let locationManager = CLLocationManager()

    func application(_ application: UIApplication, didFinishLaunchingWithOptions launchOptions: [UIApplication.LaunchOptionsKey: Any]?) -> Bool {
        // Kiểm tra và yêu cầu quyền vị trí
        locationManager.delegate = self
        checkLocationAuthorization()
        return true
    }

    func checkLocationAuthorization() {
        let status = CLLocationManager.authorizationStatus()
        switch status {
        case .notDetermined:
            print("🔴 Location: Not determined → requesting...")
            locationManager.requestWhenInUseAuthorization()
        case .restricted, .denied:
            print("⛔️ Location: Denied or restricted")
        case .authorizedAlways:
            print("✅ Location: Authorized always")
        case .authorizedWhenInUse:
            print("✅ Location: Authorized when in use")
        @unknown default:
            print("❓ Location: Unknown status")
        }
    }

    // Ghi nhận khi quyền vị trí thay đổi
    func locationManager(_ manager: CLLocationManager, didChangeAuthorization status: CLAuthorizationStatus) {
        print("📍 Location auth changed → new status: \(status.rawValue)")
    }

    func applicationWillResignActive(_ application: UIApplication) {}

    func applicationDidEnterBackground(_ application: UIApplication) {}

    func applicationWillEnterForeground(_ application: UIApplication) {}

    func applicationDidBecomeActive(_ application: UIApplication) {}

    func applicationWillTerminate(_ application: UIApplication) {}

    func application(_ app: UIApplication, open url: URL, options: [UIApplication.OpenURLOptionsKey: Any] = [:]) -> Bool {
        return ApplicationDelegateProxy.shared.application(app, open: url, options: options)
    }

    func application(_ application: UIApplication, continue userActivity: NSUserActivity, restorationHandler: @escaping ([UIUserActivityRestoring]?) -> Void) -> Bool {
        return ApplicationDelegateProxy.shared.application(application, continue: userActivity, restorationHandler: restorationHandler)
    }
}

