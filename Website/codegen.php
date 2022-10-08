<?php
session_start();

require("info.php");

// Connects to the temp database
// host: 10.75.35.226
// user: api
// pass: testpass
// db:   iml

$host2 = "10.75.35.226";
$user2 = "api";
$pass2 = "testpass";
$db2 = "iml";

// Connect using mysqli with authentication method cached_sha2_password
$mysqli2 = mysqli_connect($host2, $user2, $pass2, $db2) or die("Error " . mysqli_error($link));

if (mysqli_connect_errno($mysqli2)) {
    echo "Failed to connect to MySQL: " . mysqli_connect_error();
    die;
}


if (!isset($_SESSION['user_email_address'])) {
    header('Location: index.php');
}

if (isset($_REQUEST['cancel'])) {
    header('Location: index.php');
}

if (!isset($_SESSION['position'])) {
    header('Location: index.php');
}

if (!isset($_SESSION['authcode'])) {
    $_SESSION['authcode'] = createAuthCode($_SESSION['userId'], $mysqli2);
}

if (!isset($_SESSION['time'])) {
    $_SESSION['time'] = time() + 30;
}

if ($_SESSION['time'] < time()) {
    $_SESSION['authcode'] = createAuthCode($_SESSION['userId'], $mysqli2);
    $_SESSION['time'] = time() + 30;
}

// Function to create an auth code given the userID
function createAuthCode($userID, $mysqli2) {
    try {
        // If the user has an auth code, delete it
        $sql = "DELETE FROM token WHERE userID = ?";
        $stmt = $mysqli2->prepare($sql);
        $stmt->bind_param("s", $userID);
        $stmt->execute();
    } catch (Throwable $t) {
        echo "Error: " . $t->getMessage();
    }

    try {
        $authCode = rand(100000, 999999);
        $time = time();
        $sql = "INSERT INTO token (token, userID, timecreated) VALUES (?, ?, FROM_UNIXTIME(?))";
        $stmt = $mysqli2->prepare($sql);
        $stmt->bind_param("ssi", $authCode, $userID, $time);
        $stmt->execute();
        $stmt->close();
        return $authCode;
    } catch (Throwable $e) {
        echo "Error: " . $e->getMessage();
    }
    return $authCode;
}
?>
<!DOCTYPE html>
<html lang="en">

    <head>
        <meta charset="UTF-8">
        <title>Generate login code</title>
        <script src="https://code.jquery.com/jquery-3.4.1.min.js" integrity="sha256-CSXorXvZcTkaix6Yvo6HppcZGetbYMGWSFlBw8HfCJo=" crossorigin="anonymous"></script>
        <script src="https://unpkg.com/sweetalert/dist/sweetalert.min.js"></script>
        <script src="scripts/userAuthentication.js"></script>
        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@3.3.7/dist/css/bootstrap.min.css"
        integrity="sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u" crossorigin="anonymous">
        <script src="https://code.jquery.com/jquery-3.4.1.min.js"
        integrity="sha256-CSXorXvZcTkaix6Yvo6HppcZGetbYMGWSFlBw8HfCJo=" crossorigin="anonymous"></script>
        <script src="https://stackpath.bootstrapcdn.com/bootstrap/3.4.0/js/bootstrap.min.js"></script>
        <script src="scripts/editAccount.js"></script>
        <link href="https://stackpath.bootstrapcdn.com/bootstrap/3.4.0/css/bootstrap.min.css" rel="stylesheet" integrity="sha384-PmY9l28YgO4JwMKbTvgaS7XNZJ30MK9FAZjjzXtlqyZCqBY6X6bXIkM++IkyinN+" crossorigin="anonymous">
        <link href="css/home.css" rel="stylesheet">
        <link href="css/editAccount.css" rel="stylesheet">
    </head>

    <body>
        <!-- Displays the user's email from the session variable -->
        <h1 class="text-center">Hello <?php echo $_SESSION['user_email_address']; ?></h1>
        <h2 class="text-center">Your code is <?php echo $_SESSION['authcode']; ?></h2>
        <h2 class="text-center">Your code will expire in <?php echo $_SESSION['time'] - time(); ?> seconds</h2>
        <!-- Adds a button that logs the user out -->
        <form action="logout.php" method="post">
            <input type="submit" name="logout" value="Logout" class="btn btn-primary btn-lg btn-block">
        </form>
    </body>
</html>