<?php
session_start();

require("info.php");

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
    $_SESSION['authcode'] = rand(100000, 999999);
}

if (!isset($_SESSION['time'])) {
    $_SESSION['time'] = time() + 30;
}

if ($_SESSION['time'] < time()) {
    $_SESSION['authcode'] = rand(100000, 999999);
    $_SESSION['time'] = time() + 30;
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