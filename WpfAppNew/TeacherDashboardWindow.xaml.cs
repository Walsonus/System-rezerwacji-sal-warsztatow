using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using ReservationSystem.Core;

namespace WpfAppNew
{
    public partial class TeacherDashboardWindow : MetroWindow
    {
        private readonly IUserService _userService;
        private readonly IRoomService _roomService;

        public TeacherDashboardWindow(IUserService userService, IRoomService roomService)
        {
            InitializeComponent();
            _userService = userService;
            _roomService = roomService;
        }
    }
}