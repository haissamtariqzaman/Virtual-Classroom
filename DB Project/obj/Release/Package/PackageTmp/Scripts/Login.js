function inputValidation()
{
    var user = document.getElementById("email").value;
    var password = document.getElementById("password").value;

    var found = 0;

    for (x = 0; x < user.length; x++)
    {
        if (user[x] == '@')
        {
            found = 1;
            break;
        }
    }

    if (found == 0)
    {
        alert("Invalid email!");
        return false;
    }

    if (password.length < 8)
    {
        alert("Invalid password!")
        return false;
    }

    return true;
}