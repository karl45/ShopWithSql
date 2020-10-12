new Vue({
    el: "#form",
    data: {
        Email: '',
        Password: '',
        PasswordConfirm: '',
        visible: false,
        errors: []
    },
    methods: {
        SendData: function () {
            this.errors = [];
            error_list = this.errors
            this.visible = true;
            const request = {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ Email: this.Email, Password: this.Password, PasswordConfirm:this.PasswordConfirm })
            };
            fetch("/Account/Register", request)
                .then(response => {
                    if (response.status != 200) {
                    var x = response.json().then(function (data) {
                        for (var index = 0; index < data.length;index++) {
                            var error_element = data[index];
                            error_list.push({ error: error_element.description, code: error_element.code })
                        }
                    });
                        this.visible = false;
                }
                    else {
                        location.href = "/Goods/Index"; // к сожалению изначально не настроил vue-route
                        this.visible = false;
                    }
                })
                .then(function (text) {                          
                    console.log('Request successful', text);
                    this.visible = false;

            })
                .catch(function (error) {                       
                    console.log('Request failed', error);
                    this.visible = false;
                });
        }
    }
})